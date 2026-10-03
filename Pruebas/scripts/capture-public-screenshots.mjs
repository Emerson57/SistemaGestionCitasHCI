/**
 * Public pages only — reliable for Phase 7.2 evidence (no auth).
 * Desktop 1440×900 + mobile 375×812 home.
 */
import { chromium } from 'playwright';
import { mkdir } from 'fs/promises';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.join(__dirname, '..', 'Evidencias', 'Screenshots');
const base = process.env.WEB_BASE_URL ?? 'https://localhost:7171';

await mkdir(outDir, { recursive: true });

const browser = await chromium.launch({ headless: true, ignoreHTTPSErrors: true });

const desktop = await browser.newContext({ viewport: { width: 1440, height: 900 } });
const page = await desktop.newPage();

await page.goto(`${base}/`, { waitUntil: 'load' });
await page.waitForTimeout(1500);
await page.screenshot({ path: path.join(outDir, 'E01-home.png'), fullPage: true });

await page.goto(`${base}/login`, { waitUntil: 'load' });
await page.waitForSelector('#email', { timeout: 60000 });
await page.waitForTimeout(1500);
await page.screenshot({ path: path.join(outDir, 'E02-login.png'), fullPage: true });

await page.goto(`${base}/registro`, { waitUntil: 'load' });
await page.waitForSelector('#fullName', { timeout: 60000 });
await page.waitForTimeout(1500);
await page.screenshot({ path: path.join(outDir, 'E03-registration.png'), fullPage: true });

await desktop.close();

const mobile = await browser.newContext({ viewport: { width: 375, height: 812 } });
const mpage = await mobile.newPage();
await mpage.goto(`${base}/`, { waitUntil: 'load' });
await mpage.waitForTimeout(1500);
await mpage.screenshot({ path: path.join(outDir, 'E14-mobile.png'), fullPage: true });
await mobile.close();

await browser.close();
console.log('Public screenshots E01, E02, E03, E14 updated.');
