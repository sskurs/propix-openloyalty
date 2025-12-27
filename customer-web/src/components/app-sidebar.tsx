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
import * as React from 'react';
import { SidebarNavItem } from './sidebar-nav-item';
import { ChevronLeft, LayoutDashboard, Gift, History, Wallet, Award, Zap, Library, Search, BadgePercent, ListOrdered, TrendingUp, RefreshCw, Star } from 'lucide-react';

const customerNavItems = [
    {
        title: 'Customer Dashboard',
        url: '/dashboard',
        icon: LayoutDashboard,
        items: [
            {
                title: 'Points balance',
                url: '/dashboard/points',
            },
            {
                title: 'Tier status',
                url: '/dashboard/tier',
            },
            {
                title: 'Active campaigns',
                url: '/dashboard/campaigns',
            },
            {
                title: 'Rewards catalog',
                url: '/dashboard/catalog',
            },
        ],
    },
    {
        title: 'Rewards Redemption',
        url: '/redemption',
        icon: Gift,
        items: [
            {
                title: 'Browse rewards',
                url: '/redemption/browse',
            },
            {
                title: 'Redeem',
                url: '/redemption/redeem',
            },
            {
                title: 'View redemption history',
                url: '/redemption/history',
            },
        ],
    },
    {
        title: 'Activity History',
        url: '/activity',
        icon: History,
        items: [
            {
                title: 'Earn events',
                url: '/activity/earn',
            },
            {
                title: 'Redemptions',
                url: '/activity/redeem',
            },
            {
                title: 'Campaign Usage',
                url: '/activity/usage',
            },
            {
                title: 'Tier changes',
                url: '/activity/tier',
            },
            {
                title: 'POS Simulator',
                url: '/simulator/pos',
            },
        ],
    },

];

export default function AppSidebar() {
    return (
        <Sidebar collapsible='icon' className='group !border-r-0'>
            <SidebarHeader className='h-[65.2px] border-b-2 border-white flex items-start px-4 pt-4 bg-background/95 backdrop-blur-sm group-data-[state=collapsed]:items-center group-data-[state=collapsed]:h-[var(--header-height-collapsed)] group-data-[state=collapsed]:justify-center group-data-[state=collapsed]:p-0 transition-all'>
                <div className="flex items-start gap-2">
                    <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary text-primary-foreground group-data-[state=collapsed]:h-8 group-data-[state=collapsed]:w-8 transition-all">
                        <Star className="h-6 w-6" />
                    </div>
                    <div className='group-data-[state=collapsed]:hidden'>
                        <p className="font-bold text-2xl bg-gradient-to-r from-amber-400 to-stone-800 bg-clip-text text-transparent">LoyaltyMax</p>
                        <p className="text-[10px] text-muted-foreground -mt-1 uppercase tracking-wider font-semibold">Customer Portal</p>
                    </div>
                </div>
            </SidebarHeader>
            <SidebarContent className='bg-card'>
                <ScrollArea className="h-full custom-scroll">
                    <SidebarGroup>
                        <SidebarMenu className='mt-8'>
                            {customerNavItems.map((item) => (
                                <SidebarNavItem key={item.title} item={item} />
                            ))}
                        </SidebarMenu>
                    </SidebarGroup>
                </ScrollArea>
            </SidebarContent>
            <SidebarFooter className="flex items-center justify-end p-2 border-t">
                <SidebarTrigger className="group-data-[state=collapsed]:-rotate-180 transition-transform">
                    <ChevronLeft />
                </SidebarTrigger>
            </SidebarFooter>
        </Sidebar>
    );
}
