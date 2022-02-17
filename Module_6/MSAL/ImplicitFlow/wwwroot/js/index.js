const tenantID = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
const clientID = "0c5d75ca-66aa-479b-b39b-2b3ad6f93831";
const scopes = ["user.read", "profile", "openid", "email"];   // Make sure you consented these permissions in AAD
const redirectUrl = `https://localhost:${location.port}/index.html`;  // Register this redirect Uri in AAD
const tenantUrl = `https://login.microsoftonline.com/${tenantID}/oauth2/v2.0/`;
const commonUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/";
const organizationUrl = "https://login.microsoftonline.com/organizations/oauth2/v2.0/";

const a = document.getElementById("login")
a.href = `${tenantUrl}authorize?client_id=${clientID}&response_type=token&scope=${scopes.join(" ")}&response_mode=fragment&redirect_url=${redirectUrl}&state=12345&nonce=678910`;

const btn = document.getElementById("call-graph");
btn.addEventListener("click", async () => {
    await callGraph();
}, false);
if (localStorage.token == undefined) {
    btn.disabled = true;
}

async function callGraph() {
    let token = JSON.parse(localStorage.token);
    console.log(token.access_token);

    let reqInf = {
        "method": "GET",
        "headers": {
            "Authorization": `Bearer ${token.access_token}`
        }
    };

    let response = await fetch("https://graph.microsoft.com/v1.0/me", reqInf);
    if (response.ok) {
        let data = await response.json();
        writeTo("result", data);
    }
}
export function parseToken() {
    if (localStorage.token != undefined) return;

    let fragmentString = location.hash.substring(1);

    let params = {};
    let regex = /([^&=]+)=([^&]*)/g, m;
    while (m = regex.exec(fragmentString)) {
        params[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
    }
    if (Object.keys(params).length > 0) {
        localStorage.setItem('token', JSON.stringify(params));
        a.hidden = true;
        btn.disabled = false;
        return true;
    }
    return false;
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


