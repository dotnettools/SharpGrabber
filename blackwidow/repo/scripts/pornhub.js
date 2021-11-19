const urlMatcher = /^(https?:\/\/)?(www\.)?pornhub\.com\/([^\/]+)viewkey=(\w+).*$/i
const flashVarsFinder = /^\s*(var|let)\s+(flashvars[\w_]+)\s+=/mi

const getViewId = uri => {
	const url = new URL(uri)
	const match = urlMatcher.exec(uri)
	if (!match)
		return undefined
	return match[4]
}

const getStdUrl = url => {
	return `https://www.pornhub.com/view_video.php?viewkey=${url}`
}

const parseFlashVarsScript = doc => {
	let source
	let varName
	doc.selectAll('script').forEach(elem => {
		const match = flashVarsFinder.exec(elem.innerText)
		if (match) {
			source = elem.innerText
			varName = match[2]
		}
	})
	
	const flashVars = new Function('let playerObjList = {};'+source + ';return '+varName+';')()
	if (!flashVars)
		throw new GrabException('Could not extract flashVars.')
	return flashVars
}

const updateResult = (result, vars) => {
	const parseBool = str => typeof str === 'boolean' ? str : new Function('return ' + str)();

	if (parseBool(vars.video_unavailable))
		throw new GrabException('This video is unavailable.')
	if (parseBool(vars.video_unavailable_country))
		throw new GrabException('This video is unavailable in your country.')
	
	const duration = vars.video_duration * 1000 // milliseconds
	
	result.title = vars.video_title
	
	result.grab('info', {
		length: duration
	})
	
	result.grab('image', {
		resourceUri: vars.image_url,
		type: 'primary'
	})
	
	vars.mediaDefinitions.forEach(def => {
		if (!def.quality || def.remote || !def.videoUrl)
			return
		
		if (def.format === 'hls') {
			// grab HLS stream
			if (Array.isArray(def.quality)) {
				result.grab('hlsStreamReference', {
					resourceUri: def.videoUrl,
					playlistType: 'master',
					resolution: def.quality.join(',')
				})
			} else {
				result.grab('hlsStreamReference', {
					resourceUri: def.videoUrl,
					playlistType: 'stream',
					resolution: def.quality
				})
			}
		} else {
			// grab mp4 video
			result.grab('media', {
				resourceUri: def.videoUrl,
				format: {
					mime: 'video/mp4',
					extension: 'mp4',
					channels: 'both',
					length: duration,
					container: 'mp4,'
					resolution: def.quality,
					formatTitle: 'MP4 ' + def.quality,
				}
			})
		}
	})
}

grabber.supports = uri => {
	return getViewId(uri) !== undefined
}

grabber.grab = (request, result) => {
	
	// init
	const viewId = getViewId(request.url)
	if (!viewId)
		return false
	
	// download page
	const url = getStdUrl(viewId)
	const response = http.client.get({
		url
	})
	response.assertSuccess()
	
	// parse response HTML
	const doc = html.parse(response.bodyText)
	const flashVars = parseFlashVarsScript(doc)
	updateResult(result, flashVars)
	
	return true
}
