console.log("WinUIEdge is injecting");
const ovserver = new MutationObserver(function(mutationsList) {
    for (let mutation of mutationsList) {
        if (mutation.type === 'childList') {
            const addedNodes = mutation.addedNodes;
            for (let i = 0; i < addedNodes.length; i++) {
                const addedNode = addedNodes[i];
                if (addedNode.role === 'main') {
                    const originalInstallButton = document.querySelector("button[id^=installButton-]");
                    const installButtonParent = originalInstallButton?.parentElement?.parentElement;
                    if (!installButtonParent) {
                        return;
                    }
                    const styleElement = document.createElement("style");
                    styleElement.innerText = `#injectInstallButton {
    background-color: var(--colorBrandBackground);
    border: var(--strokeWidthThin) solid var(--colorNeutralStroke1);
    border-radius: 4px;
    box-sizing: border-box;
    cursor: pointer;
    font-family: var(--fontFamilyBase);
    font-size: var(--fontSizeBase300);
    font-weight: 600;
    height: fit-content;
    padding: 0;
    outline-style: none;
    transition: background var(--durationFaster) var(--curveEasyEase);
    &>a {
        color: var(--colorNeutralForegroundOnBrand);
        display: block;
        line-height: var(--lineHeightBase300);
        padding: 5px var(--spacingHorizontalM);
        text-decoration: none;
    }
    &:hover {
        background-color: var(--colorBrandBackgroundHover);
    }
}
    `;
                    document.head.appendChild(styleElement);
                    const installAnchorElement = document.createElement("a");
                    installAnchorElement.href = `https://edge.microsoft.com/extensionwebstorebase/v1/crx?response=redirect&x=id%3D${originalInstallButton.id.slice(14)}%26installsource%3Dondemand`;
                    installAnchorElement.innerText = "安装到 WinUIEdge";
                    installAnchorElement.download;
                    const installButtonElement = document.createElement("button");
                    installButtonElement.id = "injectInstallButton";
                    installButtonElement.appendChild(installAnchorElement);
                    installButtonParent.appendChild(installButtonElement);
                    ovserver.disconnect();
                }
            }
        }
    }
});
ovserver.observe(document.getElementById("root"), { childList: true, subtree: true });