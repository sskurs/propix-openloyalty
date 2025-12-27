import type { Metadata } from 'next';
import React from 'react';

export const metadata: Metadata = {
  title: 'Dashboard - Open Loyalty Program',
  description: 'Admin dashboard for loyalty program management'
};

export default function DashboardLayout({
  children
}: {
  children: React.ReactNode;
}) {
  return <>{children}</>;
}
