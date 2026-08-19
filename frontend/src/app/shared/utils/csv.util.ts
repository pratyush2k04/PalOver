export function escapeCsvField(value: string): string {
  if (/[",\n]/.test(value)) {
    return `"${value.replace(/"/g, '""')}"`;
  }
  return value;
}

export function downloadCsv(rows: string[][], header: string[], filenamePrefix: string): void {
  const lines = [header.join(',')];
  for (const row of rows) {
    lines.push(row.map(escapeCsvField).join(','));
  }

  const csv = lines.join('\n');
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  const date = new Date().toISOString().slice(0, 10);
  link.href = url;
  link.download = `${filenamePrefix}-${date}.csv`;
  link.click();
  URL.revokeObjectURL(url);
}
