
const TOKEN_KEY = 'wms_auth_token';
const USER_KEY = 'wms_auth_user';

export const getToken = (): string | null => {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem(TOKEN_KEY);
};

export const setToken = (newToken: string): void => {
    if (typeof window === 'undefined') return;
    localStorage.setItem(TOKEN_KEY, newToken);
};

export const clearToken = (): void => {
    if (typeof window === 'undefined') return;
    localStorage.removeItem(TOKEN_KEY);
};



/**
 * Formats a date string into a consistent DD/MM/YYYY format.
 * This helps prevent hydration errors between server and client.
 * @param date - The date object or ISO string to format.
 * @returns A formatted date string.
 */
export const formatDate = (date: Date | string | undefined | null): string => {
  if (!date) return 'N/A';
  const d = typeof date === 'string' ? new Date(date) : date;
  if (isNaN(d.getTime())) {
    return 'Invalid Date';
  }
  const day = String(d.getDate()).padStart(2, '0');
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const year = d.getFullYear();
  return `${day}/${month}/${year}`;
};

const formatTimeOnly = (date: Date): string => {
    let hours = date.getHours();
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    const strTime = `${String(hours)}:${minutes} ${ampm}`;
    return strTime;
}

/**
 * Formats a date string into a consistent HH:MM AM/PM format.
 * @param date - The date object or ISO string to format.
 * @returns A formatted time string.
 */
export const formatTime = (date: Date | string | undefined | null): string => {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(d.getTime())) {
      return 'Invalid Time';
    }
    return formatTimeOnly(d);
};


/**
 * Formats a date string into a consistent DD/MM/YYYY, HH:MM AM/PM format.
 * This helps prevent hydration errors between server and client.
 * @param date - The date object or ISO string to format.
 * @returns A formatted date string.
 */
export const formatDateTime = (date: Date | string | undefined | null): string => {
    if (!date) return 'N/A';
    const d = typeof date === 'string' ? new Date(date) : date;
    if (isNaN(d.getTime())) {
      return 'Invalid Date';
    }
    const datePart = formatDate(d);
    const timePart = formatTimeOnly(d);
    return `${datePart}, ${timePart}`;
};

/**
 * Formats a number into a compact Indian Rupee currency string.
 * @param value - The numeric value to format.
 * @returns A formatted currency string (e.g., ₹1.2M, ₹5.5K, ₹500.00).
 */
export const formatCurrency = (value: number): string => {
    if (value >= 1_000_000) return `₹${(value / 1_000_000).toFixed(1)}M`;
    if (value >= 1_000) return `₹${(value / 1_000).toFixed(1)}K`;
    return `₹${value.toFixed(2)}`;
};