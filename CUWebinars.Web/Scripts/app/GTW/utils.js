/*
 Common utility functions.
 */

// Always run the browser detection script first, before anything else!
window.onload = browserDetect;

// Array support for the push method in IE5
if (typeof Array.prototype.push != "function") {
    Array.prototype.push = ArrayPush;
    function ArrayPush(value) { this[this.length] = value; }
}

// Event Management

/*
 FUNCTION addLoadEvent()
 Queue multiple onload events, such that they don't conflict with one another. Proper usage:
 addLoadEvent(functionName);
 addLoadEvent(function () { x = y });
 */
function addLoadEvent(func) {
    var old = window.onload;
    if (typeof window.onload != "function") {
        window.onload = func;
    } else {
        window.onload = function () {
            old();
            func();
        }
    }
}
/*
 FUNCTION getEventCatalyst()
 Returns object reference to the object that fired the specified event.
 */
function getEventCatalyst(ev) {
    ev = ev || window.event;
    return ev.target || ev.srcElement;
}
/* 
 FUNCTION getMousePos()
 Returns mouse coordinates from an event.
 */
function getMousePos(e) {
    var coords = new Object();
    coords.x = 0;
    coords.y = 0;
    if (!e) var e = window.event;
    if (e.pageX || e.pageY) {
        coords.x = e.pageX;
        coords.y = e.pageY;
    } else if (e.clientX || e.clientY) {
        coords.x = e.clientX + document.body.scrollLeft;
        coords.y = e.clientY + document.body.scrollTop;
    } else {
        return null;
    }
    if (coords.x < 0) coords.x = 0;
    if (coords.y < 0) coords.y = 0;
    return coords;
}
/* 
 FUNCTION getObjPos()
 Returns coordinates of an object relative to the entire page.
 */
function getObjPos(obj) {
    var coords = new Object();
    coords.x = 0;
    coords.y = 0;
    if (obj.offsetParent) {
        while (obj.offsetParent) {
            coords.x += obj.offsetLeft;
            coords.y += obj.offsetTop;
            obj = obj.offsetParent;
        }
    } else if (obj.x && obj.y) {
        coords.x += obj.x;
        coords.y += obj.y;
    }
    return coords;
}
/*
 FUNCTION getWindowSize()
 Get browser window's current dimensions.
 */
function getWindowSize() {
    var win = new Object();
    if (window.outerHeight) {
        win.x = window.outerWidth;
        win.y = window.outerHeight;
    } else {
        win.x = document.body.clientWidth;
        win.y = document.body.clientHeight;
    }
    return win;
}
/*
 FUNCTION addEventHandler()
 Cross-browser compatible method to add event handlers to an object.
 */
function addEventHandler(obj, eventType, func) {
    if (!obj || !eventType || !func) return;
    if (document.addEventListener) {
        if (eventType.indexOf("on") == 0) eventType = eventType.substring(2);
        obj.addEventListener(eventType, func, true);
    } else if (document.attachEvent) {
        if (eventType.indexOf("on") != 0) eventType = "on" + eventType;
        obj.attachEvent(eventType, func);
    }
}


// DOM Utilities

/*
 FUNCTION getElementsByClassName()
 Returns an array of objects that match a specified class or array of classes.
 1st arg (required): A single class (String/RegExp). Could also be an array of classes, in which case the object would match
 only if its class name contains every whole word class in the specified array.
 2nd arg (optional): If no tag is specified, all tags will be matched against specified class.
 3rd arg (optional): Defaults to document.body object if not specified.
 */
function getElementsByClassName(classSearch, tag, parentObj) {
    if (!parentObj) parentObj = document.body;
    if (!tag) tag = "*";
    var arrElements = (tag == "*" && parentObj.all) ? parentObj.all : parentObj.getElementsByTagName(tag);
    var arrReturnElements = new Array();
    var arrRegExpClassNames = new Array();
    if (classSearch instanceof RegExp) {
        arrRegExpClassNames.push(classSearch);
    } else if (typeof classSearch == "object") {
        for (var i = 0; i < classSearch.length; i++)
            arrRegExpClassNames.push(new RegExp("(^|\\s)" + classSearch[i].replace(/\-/g, "\\-") + "(\\s|$)"));
    }
    else arrRegExpClassNames.push(new RegExp("(^|\\s)" + classSearch.replace(/\-/g, "\\-") + "(\\s|$)"));
    var oElement, doesMatchAll;
    for (var j = 0; j < arrElements.length; j++) {
        oElement = arrElements[j];
        doesMatchAll = true;
        for (var k = 0; k < arrRegExpClassNames.length; k++) {
            if (!arrRegExpClassNames[k].test(oElement.className)) {
                doesMatchAll = false;
                break;
            }
        }
        if (doesMatchAll) arrReturnElements.push(oElement);
    }
    return arrReturnElements;
}
/*
 FUNCTION getParentNode()
 Returns an object reference to the parent node of a specified child object based on tag name.
 1st arg: required, tag name (string) of parent object.
 2nd arg: required, child object from which to start searching.
 3rd arg: optional, class name of parent object; can be a single string or an array of strings.
 */
function getParentNode(tag, obj, cn) {
    if (!tag) return;
    else tag = tag.toUpperCase();
    try {
        if (cn != null) {
            if (isArray(cn)) {
                if (obj.parentNode.nodeName.toUpperCase() == tag)
                    for (var i = 0; i < cn.length; i++) if (obj.parentNode.className.match(cn[i]) != -1) return obj.parentNode;
                return getParentNode(tag, obj.parentNode, cn);
            } else return ((obj.parentNode.nodeName.toUpperCase() == tag) && (obj.parentNode.className.match(cn) != -1)) ? obj.parentNode : getParentNode(tag, obj.parentNode, cn);
        } else
            return (obj.parentNode.nodeName.toUpperCase() == tag) ? obj.parentNode : getParentNode(tag, obj.parentNode);
    } catch (e) { };
    return null;
}
/*
 FUNCTION getPrevNode()
 Returns the previous, non-text sibling relative to specified node.
 */
function getPrevNode(obj) {
    do obj = obj.previousSibling;
    while (obj && obj.nodeType != 1);
    return obj;
}
/*
 FUNCTION getNextNode()
 Returns the next, non-text sibling relative to specified node.
 */
function getNextNode(obj) {
    do obj = obj.nextSibling;
    while (obj && obj.nodeType != 1);
    return obj;
}
/*
 FUNCTION createNewElement
 Creates a new element of specified type, class and id.
 */
function createNewElement(elType, elClass, elId) {
    if (!elType) return;
    var obj = document.createElement(elType);
    if (!obj) return;
    if (elClass) obj.className = elClass;
    if (elId) obj.id = elId;
    return obj;
}

// Effects

/*
 FUNCTION setOpacity()
 Sets the opacity of a specified element. (0 <= opacity <= 100)
 */
function setOpacity(el, opacity) {
    if (!el) return;
    if (opacity < 0) opacity = 0;
    else if (opacity > 100) opacity = 100;
    if (typeof el.style.opacity != "undefined") {
        el.style.opacity = opacity / 100;
    } else if (typeof el.style.MozOpacity != "undefined") {
        el.style.MozOpacity = opacity / 100;
    } else if (typeof el.style.filter != "undefined") {
        el.style.filter = "alpha(opacity=" + opacity + ")";
    }
}

// Miscellaneous

/*
 FUNCTION browserDetect()
 Stores boolean values in global vars related to browser detection.
 */
var isIE, isPreIE7, isPreIE55, isGecko, isSafari_old, isSafari_new, isKonqueror;
function browserDetect() {
    var ua = navigator.userAgent.toLowerCase();
    isIE = ((ua.indexOf("msie") != -1) && (ua.indexOf("opera") == -1) && (ua.indexOf("webtv") == -1));
    var tmp = ua.match(/MSIE\s(\d)\.(\d)/i);
    if (tmp) {
        isPreIE7 = (isIE && tmp[1] && parseInt(tmp[1]) < 7);
    } else {
        isPreIE7 = false;
    }
    isPreIE55 = (isPreIE7 && (parseInt(tmp[1]) <= 5) && tmp[2] && (parseInt(tmp[2]) < 5));
    isGecko = (ua.indexOf("gecko") != -1);
    if (ua.indexOf("safari") != -1) {
        isSafari_new = (parseInt((/AppleWebKit\/(\d+)/i).exec(ua)[1]) >= 500);
        isSafari_old = !isSafari_new;
    }
    isKonqueror = (ua.indexOf("konqueror") != -1);
}
/*
 FUNCTION HashMap()
 HashMap object prototype
 */
function HashMap() {
    this.length = 0;
    this.items = new Array();
    for (var i = 0; i < arguments.length; i += 2) {
        if (typeof (arguments[i + 1]) != "undefined") {
            this.items[arguments[i]] = arguments[i + 1];
            this.length++;
        }
    }
    this.getItem = function (in_key) {
        return this.items[in_key];
    }
    this.hasItem = function (in_key) {
        return typeof (this.items[in_key]) != "undefined";
    }
}
/*
 FUNCTION isArray()
 Returns true if the passed-in parameter is an array; returns false otherwise.
 */
function isArray(obj) {
    return (obj instanceof Array);
}
/*
 FUNCTION clearDefault()
 Clears a specified default value from a specified form field.
 */
function clearDefault(obj, txt) {
    if (!obj) return;
    if (obj.value == txt) obj.value = "";
}
/*
 FUNCTION firstFocus()
 Finds the first textarea or input of type text inside a specified parent and gives it focus.
 */
function firstFocus(obj) {
    if (!obj) obj = document.body;
    try {
        var str;
        var theList = obj.getElementsByTagName("*");
        if (!theList || theList.length < 1) theList = obj.all; // IE5 support
        for (var i = 0; i < theList.length; i++) {
            str = theList[i].nodeName.toLowerCase();
            if (str == "textarea" || ((str == "input") && (theList[i].type == "text"))) {
                theList[i].focus();
                break;
            }
        }
    } catch (e) { };
}
/*
 FUNCTION toggleSection()
 Expands/collapses a section (e.g. DIV). For use with DynamicSectionHeader macro.
 Param #1: Object reference or its id. REQUIRED.
 Param #2: Boolean to force show/hide (true/false) the section. OPTIONAL. If null is passed in,
 the section will toggle to whatever display state it's not currently in (e.g. if it's
 currently visible, a call to this function will hide that section, and vice versa).
 */
function toggleSection(obj, doShow) {
    if (obj == null) return;
    if (typeof obj == "string") obj = document.getElementById(obj);
    if (obj == null) return;
    var sectionObj = document.getElementById("section_" + obj.id);
    var imgObj = sectionObj.getElementsByTagName("img")[0];
    if (!doShow || doShow === "true" || doShow === "false") {
        if (doShow == "true") doShow = true;
        else if (doShow == "false") doShow = false;
        else doShow = !(imgObj.src.indexOf("expand") == -1);
    }
    obj.style.display = doShow ? "block" : "none";
    imgObj.src = "/images/icons/" + (doShow ? "collapse" : "expand") + ".gif";
    if (doShow) firstFocus(obj);
    return false;
}
//Standards compliant way to replace "target=_blank".
function newWindow(theURL, windowName) {
    window.open(theURL, windowName);
    return false;
}
