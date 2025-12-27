import type { Metadata } from "next";
import { cookies } from "next/headers";
import { fontVariables } from "@/lib/font";
import { cn } from "@/lib/utils";
import ThemeProvider from "@/components/theme-provider";
import { ActiveThemeProvider } from "@/components/active-theme";
import { NuqsAdapter } from 'nuqs/adapters/next/app';
import { Toaster } from "@/components/ui/sonner";
import { Toaster as ShadcnToaster } from "@/components/ui/toaster";
import NextTopLoader from 'nextjs-toploader';
import "./globals.css";
import "./theme.css";

export const metadata: Metadata = {
  title: "OpenLoyalty Customer Portal",
  description: "Manage your loyalty points and rewards",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const cookieStore = await cookies();
  const activeThemeValue = cookieStore.get('active_theme')?.value;
  const isScaled = activeThemeValue?.endsWith('-scaled');

  return (
    <html lang="en" suppressHydrationWarning>
      <body
        className={cn(
          "bg-background font-sans antialiased",
          fontVariables,
          activeThemeValue ? `theme-${activeThemeValue}` : '',
          isScaled ? 'theme-scaled' : ''
        )}
      >
        <NextTopLoader color='var(--primary)' showSpinner={false} />
        <NuqsAdapter>
          <ThemeProvider
            attribute="class"
            defaultTheme="system"
            enableSystem
            disableTransitionOnChange
            enableColorScheme
          >
            <ActiveThemeProvider initialTheme={activeThemeValue}>
              <div className="flex flex-col h-screen">
                <div className="flex-1 overflow-y-auto no-scrollbar">
                  {children}
                </div>
              </div>
              <Toaster />
              <ShadcnToaster />
            </ActiveThemeProvider>
          </ThemeProvider>
        </NuqsAdapter>
      </body>
    </html>
  );
}
