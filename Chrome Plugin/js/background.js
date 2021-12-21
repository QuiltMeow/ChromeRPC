function updatePresence(tab) {
    let data;
    if (tab) {
        let url = new URL(tab.url);
        data = {
            action: "set",
            host: url.hostname,
            title: tab.title,
            url: tab.url
        };
    } else {
        data = {
            action: "clear"
        };
    }

    let setting = {
        async: true,
        crossDomain: true,
        url: "http://localhost:8092",
        method: "POST",
        headers: {
            "content-type": "application/json"
        },
        processData: false,
        data: JSON.stringify(data)
    };
    $.ajax(setting);
}

let lastCheckTabId;
let isFocus = false;

setInterval(() => {
    chrome.windows.getLastFocused({
        populate: true
    }, (window) => {
        if (window.focused && window.tabs) {
            for (let tab of window.tabs) {
                if (tab.highlighted) {
                    if (tab.id !== lastCheckTabId || !isFocus) {
                        updatePresence(tab);
                        lastCheckTabId = tab.id;
                    }
                    break;
                }
            }
            isFocus = true;
        } else {
            if (isFocus) {
                updatePresence(null);
            }
            isFocus = false;
        }
    });
}, 1000);