const { openBrowser, write, $, closeBrowser, goto, press, screenshot, above, hover, click, checkBox, listItem, toLeftOf, link, text, into, currentURL, textBox, evaluate, below, storage: { localStorage, sessionStorage}, button } = require('taiko');
const { Guid } = require('js-guid');
const assert = require("assert");


step("Create new sheet", async () => {
    let sheetname = Guid.newGuid().toString();
    await click('Create New');
    await write(sheetname, textBox(below('Sheet Name')));
    await write(0, textBox(below('Starting Balance')));
    await click('Save');

    await localStorage.setItem("sheetname", sheetname);
});

step("Should redirect to sheet page", async () => {
    let sheetname =  await localStorage.getItem("sheetname");
    let url = await currentURL();
    assert.ok(url.includes("financesheet"));
    assert.ok(await text(sheetname).exists);
})

step("Create new <reoccurnace> income of <amount> with preemptive payment", async (reoccurance, amount) => {
    let incomeName = Guid.newGuid().toString();
    await localStorage.setItem("incomename", incomeName);
    await hover('Incomes');
    await click($('body > app-root > balance-sheet > div > div.col-9 > div > income-card > div > div:nth-child(1) > div > div.col-2 > button'));
    await write(incomeName, textBox(below('Income Name')));
    await write(amount, textBox(below('Income Amount')));
    await click(checkBox(below('Pay Premptively?')));
    await hover('Select A Type');
    await click('Work');
    await hover('Income Name');
    await hover('Select A Reoccurance Type');
    await click(reoccurance);
    await hover('Income Name');


    await click('Save');
})

step("Should have entered new income entry", async function() {
	let incomeName = await localStorage.getItem("incomename");
    assert.ok(await text(incomeName).exists());
});

step("Balance should be <amount>", async function(amount) {
	assert.ok(await text(amount, below('Your Current Balance Is')).exists());
});

step("Create new <reoccurnace> expenditure of <amount> with preemptive payment", async function(reoccurnace, amount) {
    let expenditureName = Guid.newGuid().toString();
    await localStorage.setItem("expenditureName", expenditureName);
    await hover('Expenditures');
    await click($('body > app-root > balance-sheet > div > div.col-9 > div > expenditure-card > div > div:nth-child(1) > div > div.col-2 > button'));
    await write(expenditureName, textBox(below('Expenditure Name:')));
    await write(amount, textBox(below('Expenditure Amount')));
    await click(checkBox(below('Pay Premptively?')));
    await hover('Select A Type');
    await click('Utility Bill');
    await hover('Expenditure Name');
    await hover('Select A Reoccurance Type');
    await click(reoccurnace);
    await hover('Expenditure Name');

    await click('Save');
});


step("Should have entered new expenditure entry", async function() {
    let expenditureName = await localStorage.getItem("expenditureName");
    assert.ok(await text(expenditureName).exists());
});

step("Create new goal of <amount> that ends today", async function(amount) {
	let goalName = Guid.newGuid().toString();
    await localStorage.setItem("goalName", goalName);
    await hover('Saving Goals');
    await click($('body > app-root > balance-sheet > div > div.col-9 > div > goal-card > div > div:nth-child(1) > div > div.col-2 > button'));
    await write(goalName, textBox(below('Goal Name')));
    await write(amount, textBox(below('Goal Amount')));
    await click($('#endDate'));
    await press('Space');
    await press('Enter')
    await click('Save');
});

step("Deposit <amount> into goal", async function(amount) {
    let name = await localStorage.getItem("goalName");
	await hover(name);
    await click($('body > app-root > balance-sheet > div > div.col-9 > div > goal-card > div > div:nth-child(2) > div > div > div > div.col-4 > div > button:nth-child(1)'));
    await write(amount, textBox(above('Deposit')));
    await press('Enter');
});

step("Should have entered new goal entry", async function() {
    let name = await localStorage.getItem("goalName");
    assert.ok(await text(name).exists());
});