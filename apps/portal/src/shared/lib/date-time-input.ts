export function toDateTimeLocalInputValue(date = new Date()) {
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return localDate.toISOString().slice(0, 16);
}

export function dateTimeLocalInputToUtc(value: string) {
  return new Date(value).toISOString();
}
