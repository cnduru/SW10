function showInfoList(e){
    var siblings = e.parentNode.childNodes;
    var len = siblings.length;

    for(var i = 0; i < len; i++){
        var sibling = siblings[i];
    	if (sibling.nodeName == "UL"
	    && sibling.className == "clickable"){
	    var style = sibling.style;
	    if (style.visibility != "visible"){
		style.position = "static";
                style.visibility = "visible";
	    } else{
		style.visibility = "hidden";
                style.position = "absolute";
	    }
	}
    }
}