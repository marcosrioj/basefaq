import fs from "node:fs";
import path from "node:path";

const localeDirectory = path.resolve("src/shared/lib/i18n/locales");
const localeFiles = fs
  .readdirSync(localeDirectory)
  .filter((file) => file.endsWith(".json"))
  .sort();
const defaultLocale = "en-US.json";
const defaultMessages = JSON.parse(
  fs.readFileSync(path.join(localeDirectory, defaultLocale), "utf8"),
);
const defaultKeys = Object.keys(defaultMessages);
const defaultKeySet = new Set(defaultKeys);
const failures = [];

function placeholders(value) {
  return [...value.matchAll(/\{\w+\}/g)].map((match) => match[0]).sort();
}

for (const file of localeFiles) {
  const messages = JSON.parse(
    fs.readFileSync(path.join(localeDirectory, file), "utf8"),
  );
  const keys = Object.keys(messages);
  const keySet = new Set(keys);
  const missing = defaultKeys.filter((key) => !keySet.has(key));
  const extra = keys.filter((key) => !defaultKeySet.has(key));

  if (missing.length) {
    failures.push(`${file} is missing: ${missing.join(", ")}`);
  }
  if (extra.length) {
    failures.push(`${file} has extra keys: ${extra.join(", ")}`);
  }

  for (const key of defaultKeys) {
    const value = messages[key];
    if (typeof value !== "string" || !value.trim()) {
      failures.push(`${file} has an empty or non-string value for: ${key}`);
      continue;
    }

    const expectedPlaceholders = placeholders(key);
    const actualPlaceholders = placeholders(value);
    if (expectedPlaceholders.join("|") !== actualPlaceholders.join("|")) {
      failures.push(
        `${file} has mismatched placeholders for: ${key} (expected ${expectedPlaceholders.join(", ") || "none"}; received ${actualPlaceholders.join(", ") || "none"})`,
      );
    }
  }
}

if (failures.length) {
  console.error(failures.join("\n"));
  process.exitCode = 1;
} else {
  console.log(
    `${localeFiles.length} locale catalogs share ${defaultKeys.length} valid keys.`,
  );
}
