grabber.supports = function (url) {
	return false
}

grabber.grab = function (a, b, resolve, reject) {
	reject()
	return false
}