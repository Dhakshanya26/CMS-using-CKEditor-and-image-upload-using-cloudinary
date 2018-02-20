component name="ImageBrowser"
	output="false"
{
	remote array function list(required string dir)
		output="false"
		returnformat="JSON"
	{
		debugger;
		var length = 0;
		var path = var image = var folder = "";
		var result = [];
		var dirList = directoryList(expandPath("./#arguments.dir#"), true, "path");

		arrayEach(dirList, function(x) {
			length = listLen(x, "\/");
			if (reFind("\w.*\.(jpg|png|gif|bmp)", listGetAt(x, length, "\/"))) {
				image = listGetAt(x, length, "\/");
				path = reMatch("(#listGetAt(dir, 1, "\/")#).*", reReplace(listDeleteAt(x, length, "\/"), "\\", "/", "ALL"))[1];
				folder = listGetAt(x, length - 1, "\/");
				arrayAppend(result, {"image": "/#path#/#image#", "folder": "#folder#"});
			}
		});

		return result;
	}
}