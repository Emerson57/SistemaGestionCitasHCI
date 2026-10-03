import { chromium } from 'playwright';
import { mkdir } from 'fs/promises';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.join(__dirname, '..', 'Evidencias', 'Screenshots');
const base = 'https://localhost:7171';

await mkdir(outDir, { recursive: true });

const browser = await chromium.launch({ ignoreHTTPSErrors: true });
const context = await browser.newContext({ viewport: { width: 1280, height: 900 } });
const page = await context.newPage();

await page.goto(base + '/');
await page.waitForTimeout(2000);
await page.screenshot({ path: path.join(outDir, 'E01-home.png'), fullPage: true });

await page.goto(base + '/login');
await page.waitForSelector('#email', { timeout: 60000 });
await page.waitForTimeout(3000);
await page.screenshot({ path: path.join(outDir, 'E02-login.png'), fullPage: true });

await page.goto(base + '/registro');
await page.waitForTimeout(1500);
await page.screenshot({ path: path.join(outDir, 'E03-registration.png'), fullPage: true });

const email = process.env.DEV_PATIENT_EMAIL ?? 'patient@dev.local';
const password = process.env.DEV_PATIENT_PASSWORD;
if (!password) {
  console.error('Set DEV_PATIENT_PASSWORD (from User Secrets) — no committed defaults.');
  process.exit(1);
}
await page.fill('#email', email);
await page.fill('#password', password);
await page.locator('button.btn-primary').click();
try {
  await page.waitForURL(/\/paciente/, { timeout: 45000, waitUntil: 'networkidle' });
  await page.waitForTimeout(2500);
  await page.screenshot({ path: path.join(outDir, 'E04-patient-dashboard.png'), fullPage: true });
  await page.goto(base + '/paciente/citas/nueva');
  await page.waitForTimeout(4000);
  await page.screenshot({ path: path.join(outDir, 'E05-specialty-selection.png'), fullPage: true });
} catch (e) {
  await page.screenshot({ path: path.join(outDir, 'E04-login-failed-debug.png'), fullPage: true });
  console.warn('Patient login screenshot flow incomplete:', e.message);
}

await page.setViewportSize({ width: 375, height: 812 });
await page.goto(base + '/');
await page.waitForTimeout(1500);
await page.screenshot({ path: path.join(outDir, 'E14-mobile.png'), fullPage: true });

await browser.close();
console.log('Screenshots saved to', outDir);
