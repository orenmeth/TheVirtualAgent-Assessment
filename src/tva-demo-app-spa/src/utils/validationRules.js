import { usePersonsStore } from 'src/stores/personsStore.js';

export const isRequired = (val) => {
  if (val === null || val === undefined) return 'This field is required.';
  const strVal = String(val).trim();
  return strVal.length > 0 || 'This field is required.';
};

export const isNotFutureDate = (val) => {
  if (!val) {
    return true;
  }
  const selectedDate = new Date(val);
  if (isNaN(selectedDate.getTime())) {
    return 'Invalid date format.';
  }
  selectedDate.setHours(0, 0, 0, 0);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return selectedDate <= today || 'The selected date cannot be in the future.';
};

export const isMonetaryValue = (val) => {
  if (val === null || val === undefined || String(val).trim() === '') {
    return true;
  }

  const valueStr = String(val);

  const moneyRegex = /^[-+]?\d*(\.\d{0,4})?$/;

  if (!moneyRegex.test(valueStr)) {
    return 'Invalid format. Use numbers, an optional decimal point, and up to 4 decimal places (e.g., 123.45 or -50.0000).';
  }

  if (['.', '-.', '+.', '-', '+'].includes(valueStr)) {
      return 'Invalid number.';
  }

  const num = parseFloat(valueStr);
  if (isNaN(num) && valueStr.trim() !== '') {
      return 'Not a valid number.';
  }

  if (num === 0) {
    return 'Value cannot be zero.';
  }

  const minValue = BigInt('-9223372036854775808');
  const maxValue = BigInt('9223372036854775807');
  if (num < minValue || num > maxValue) {
    return `Value must be between ${minValue} and ${maxValue}.`;
  }

  return true;
};

export const isNotMoreThanMaxLength = (maxLength) => (val) => {
  if (!val) return true;
  return String(val).length <= maxLength || `Length cannot exceed ${maxLength} characters.`;
}

export const isNotExistingIdNumber = (personCode) => (val) => {

const personsStore = usePersonsStore()
  const existing = personsStore.persons.find((person) => person.idNumber === val && person.code !== personCode);
  if (existing) {
    return `ID number ${val} already exists.`;
  }
  return true;
}
