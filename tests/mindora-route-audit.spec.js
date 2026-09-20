const { test, expect } = require('@playwright/test');

const routes = [
  '/',
  '/Habit',
  '/Journal',
  '/Assessment',
  '/Community',
  '/Notification',
  '/Consultation',
  '/Professional',
  '/Profile',
  '/Payment',
  '/AI',
  '/Admin',
  '/AdminDashboard',
  '/Report',
  '/Presence'
];

for (const route of routes) {

  test(`Route Audit ${route}`, async ({ page }) => {

    const consoleErrors = [];

    page.on('console', msg => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });

    page.on('pageerror', err => {
      consoleErrors.push(err.message);
    });

    const response = await page.goto(
      `http://localhost:5000${route}`,
      {
        waitUntil: 'networkidle'
      });

    expect(response.status()).toBeLessThan(500);

    if (consoleErrors.length > 0) {
      console.log(`ERRORS FOUND IN ${route}`);
      console.log(consoleErrors);
    }

    await page.screenshot({
      path: `QA-Reports/${route.replace(/\//g, '_') || 'home'}.png`,
      fullPage: true
    });
  });
}