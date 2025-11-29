import KBar from '@/components/kbar';
import AppSidebar from '@/components/layout/app-sidebar';
import Header from '@/components/layout/header';
import { SidebarInset, SidebarProvider } from '@/components/ui/sidebar';
import type { Metadata } from 'next';
import { cookies } from 'next/headers';
import React from 'react';

export const metadata: Metadata = {
  title: 'Next Shadcn Dashboard Starter',
  description: 'Basic dashboard with Next.js and Shadcn'
};

export default async function DashboardLayout({
  children
}: {
  children: React.ReactNode;
}) {
  const cookieStore = await cookies();
  const defaultOpen = cookieStore.get("sidebar_state")?.value === "true"
  return (
    <KBar>
      <SidebarProvider defaultOpen={defaultOpen} style={{ '--header-height': '59px' } as React.CSSProperties}>
        <AppSidebar />
        <SidebarInset className="flex flex-col h-screen">
          <Header />
          <div className="flex-1 overflow-hidden p-4">
            {children}
          </div>
        </SidebarInset>
      </SidebarProvider>
    </KBar>
  );
}
