const urlRegex = /^https?:\/\/(www\.|player\.)?vimeo\.com\/(video\/)?([0-9]+)/

function getVideoId(url) {
	const match = urlRegex.exec(url)
	return match ? match[3] : undefined
}

function getConfigUrl(videoId) {
	return 'https://player.vimeo.com/video/{0}/config'.replace('{0}', videoId)
}

function fetchConfig(videoId) {
	const url = getConfigUrl(videoId)
	const response = http.client.get({
		url,
		expectText: true
	})
	response.assertSuccess()
	return JSON.parse(response.bodyText)
}

function setGrabResult(result, config) {
	if (!config.request.files)
		throw new GrabException('Video is unavailable.')
	
	// add info
	result.title = config.video.title
	result.grab('info', {
		author: config.video.owner?.name,
		length: config.video.duration * 1000,
	})
	
	// add images
	if (config.video.thumbs) {
		for (var key in config.video.thumbs) {
			const isBase = Number.isNaN(Number(key))
			const size = isBase ? undefined : {
				width: key,
				height: key * 0.5625
			};
			result.grab('image', {
				resourceUri: config.video.thumbs[key],
				type: isBase ? 'primary' : 'thumbnail',
				size
			})
		}
	}
	
	// add media
	config.request.files.progressive.forEach(file => {
		const fileMime = file.mime || 'video/mp4'
		const fileExt = mime.getExtension(fileMime)
		const containerName = fileExt.toUpperCase()
		result.grab('media', {
			resourceUri: file.url,
			channels: 'both',
			container: containerName,
			resolution: file.quality,
			formatTitle: containerName + ' ' + file.quality,
			pixelWidth: file.width,
			pixelHeight: file.height,
			format: {
				mime: fileMime,
				extension: fileExt
			}
		})
	})
}

grabber.supports = url => Boolean(getVideoId(url))

grabber.grab = (request, result) => {
	const videoId = getVideoId(request.url)
	if (!videoId)
		return false
	
	const config = fetchConfig(videoId)
	if (!config)
		throw new GrabException('Failed to fetch video config.')
	
	setGrabResult(result, config)
	return true
}