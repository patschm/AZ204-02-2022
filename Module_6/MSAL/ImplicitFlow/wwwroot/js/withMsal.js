/// <reference path="../msal/index.js" />
const tenantID = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
const clientID = "0c5d75ca-66aa-479b-b39b-2b3ad6f93831";
const scopes = ["user.read", "profile", "email"]; // Make sure you consented these permissions in AAD
const redirectUrl = `https://localhost:${location.port}/index.html`;  // Register this redirect Uri in AAD
const tenantUrl = `https://login.microsoftonline.com/${tenantID}/oauth2/v2.0/`;
const commonUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/";
const organizationUrl = "https://login.microsoftonline.com/organizations/oauth2/v2.0/";

const msalConfig = {
    auth: {
        clientId: clientID,
        authority: `https://login.microsoftonline.com/${tenantID}`,
        knownAuthorities: [],
        redirectUri: redirectUrl,
        navigateToLoginRequestUrl: true
    },
    cache: {
        cacheLocation: "localStorage",
        storeAuthStateInCookie: true
    }
}
const msalInstance = new Msal.UserAgentApplication(msalConfig);
const login =  (scope) => msalInstance.loginPopup(scope); 
const get_token = (scope) => msalInstance.acquireTokenSilent(scope);
const get_token_popup = (scope) => msalInstance.acquireTokenPopup(scope);

const a = document.getElementById("login")
a.addEventListener("click", async () => {
    let request = {
        scopes: scopes
    }
    let resp = await login(request);
    if (resp) {
        btn.disabled = false;
    }
}, false);

const btn = document.getElementById("call-graph");
btn.addEventListener("click", async () => {
    await callGraph();
}, false);
if (localStorage.token == undefined) {
    btn.disabled = true;
}

async function callGraph() {
    var tokenRequest = {
        scopes: ["user.read"]
    };
    try {
        var token = await get_token(tokenRequest);
    }
    catch
    {
        var token = await get_token_popup(tokenRequest);
    }
    console.log(token.accessToken);

    let reqInf = {
        "method": "GET",
        "headers": {
            "Authorization": `Bearer ${token.accessToken}`
        }
    };

    let response = await fetch("https://graph.microsoft.com/v1.0/me", reqInf);
    if (response.ok) {
        let data = await response.json();
        writeTo("result", data);
    }
}
function writeTo(id, data) {
    let container = document.getElementById(id);
    let frag = document.createDocumentFragment();
    for (let prop in data) {
        let h2 = document.createElement("h2");
        h2.textContent = `${prop}: ${data[prop]}`;
        frag.appendChild(h2);
    }
    container.appendChild(frag);
}
