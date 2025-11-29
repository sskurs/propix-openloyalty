'use client';
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarHeader,
  SidebarMenu,
  SidebarFooter,
  SidebarTrigger,
} from '@/components/ui/sidebar';
import { ScrollArea } from "@/components/ui/scroll-area";
import { navItems } from '@/constants/data';
import * as React from 'react';
import { SidebarNavItem } from './sidebar-nav-item';
import { IconChevronLeft } from '@tabler/icons-react';

export default function AppSidebar() {

  return (
    <Sidebar collapsible='icon' className='group !border-r-0'>
      <SidebarHeader className='h-[65.2px] border-b-2 border-white flex items-start px-4 pt-4 bg-background/95 backdrop-blur-sm group-data-[state=collapsed]:items-center group-data-[state=collapsed]:h-[var(--header-height-collapsed)] group-data-[state=collapsed]:justify-center group-data-[state=collapsed]:p-0 transition-all'>
        <div className="flex items-start gap-2">
            <img src="/logo.png" alt="Logo" className="h-10 w-10 transition-all group-data-[state=collapsed]:h-8 group-data-[state=collapsed]:w-8" />
            <div className='group-data-[state=collapsed]:hidden'>
                <p className="font-bold text-2xl bg-gradient-to-r from-amber-400 to-stone-800 bg-clip-text text-transparent">LoyaltyPro™</p>
            </div>
        </div>
      </SidebarHeader>
      <SidebarContent className='bg-card'>
        <ScrollArea className="h-full custom-scroll">
            <SidebarGroup>
            <SidebarMenu className='mt-8'>
                {navItems.map((item) => (
                <SidebarNavItem key={item.title} item={item} />
                ))}
            </SidebarMenu>
            </SidebarGroup>
        </ScrollArea>
      </SidebarContent>
      <SidebarFooter className="flex items-center justify-end p-2">
         <SidebarTrigger className="group-data-[state=collapsed]:-rotate-180 transition-transform">
            <IconChevronLeft />
        </SidebarTrigger>
      </SidebarFooter>
    </Sidebar>
  );
}
