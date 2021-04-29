const { openBrowser, write, closeBrowser, goto, press, screenshot, above, click, checkBox, listItem, toLeftOf, link, text, into, textBox, evaluate, below, storage: { localStorage, sessionStorage}, button } = require('taiko');
const { Guid } = require('js-guid');
const assert = require("assert");
step("Register new user", async () => {
    await click("register");

    let username = Guid.newGuid().toString();
    await write(username, textBox(below('Username')));
    await write("password", textBox(below('Password')));
    await write(username + "@gmail.com", textBox(below('Email')));
    await write("07534521353", textBox(below('Phone Number')));
    await write("21", textBox(below('Age:')));
    await click("register");

    await localStorage.setItem("username", username);
})

step("Should be redirected to login page", async () => {
    assert.ok(await button('login').exists());
})

step("Login to users account", async () => {
    let username = await localStorage.getItem("username");
    await write(username, textBox(below('username')));
    await write("password", textBox(below('Password')));
    await click('Login');
})

step("Should be redirected to sheet selector page", async () => {
    assert.ok(await text('Select A Finance Sheet').exists());
})