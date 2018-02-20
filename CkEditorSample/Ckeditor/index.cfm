<cfset basehref = "http://#CGI.HTTP_HOST##listDeleteAt(CGI.SCRIPT_NAME, listLen(CGI.SCRIPT_NAME, '/'), '/') & '/'#">
<cfoutput>
<!DOCTYPE html>

<html lang="en">
	
	<head>
		<title>ImageBrowser.cfc For CKEditor Image Browser Addon</title>
		<base href="#basehref#">
		<script src="//cdn.ckeditor.com/4.4.6/standard/ckeditor.js"></script>
	</head>

	<body>
		<h2>CKEditor Image Browser Plugin With ColdFusion 10</h2>
		<form style="width: 600px; margin: 20px auto;">
			<textarea id="pagecontent" name="pagecontent"></textarea>
		</form>
		<script type="text/javascript">
			CKEDITOR.plugins.addExternal( 'imagebrowser', '#basehref#assets/js/ckeditor/plugins/imagebrowser/', 'plugin.js' );
			CKEDITOR.replace( 'pagecontent', {
		        extraPlugins: "imagebrowser",
		        imageBrowser_listUrl: "#basehref#ImageBrowser.cfc?method=list&dir=assets/images"
			});
		</script>
	</body>

</html>
</cfoutput>