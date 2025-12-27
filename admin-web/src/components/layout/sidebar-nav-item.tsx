'use client';
import { NavItem } from '@/types';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import * as React from 'react';
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger
} from '@/components/ui/collapsible';
import {
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
} from '@/components/ui/sidebar';
import { IconChevronRight, IconLayoutDashboard, IconUsers, IconWallet, IconAward, IconSpeakerphone, IconGift, IconTrophy, IconTicket, IconMail, IconChartBar, IconSettings } from '@tabler/icons-react';

interface SidebarNavItemProps {
  item: NavItem;
}

const Icon = ({ name }: { name: string }) => {
  const icons: { [key: string]: React.ReactNode } = {
    dashboard: <IconLayoutDashboard className="text-sky-500" />,
    user: <IconUsers className="text-violet-500" />,
    wallet: <IconWallet className="text-green-500" />,
    award: <IconAward className="text-amber-500" />,
    campaign: <IconSpeakerphone className="text-rose-500" />,
    gift: <IconGift className="text-blue-500" />,
    trophy: <IconTrophy className="text-yellow-500" />,
    coupon: <IconTicket className="text-lime-500" />,
    mail: <IconMail className="text-pink-500" />,
    chart: <IconChartBar className="text-teal-500" />,
    settings: <IconSettings className="text-slate-500" />,
  };

  return icons[name] || null;
}

export function SidebarNavItem({ item }: SidebarNavItemProps) {
  const pathname = usePathname();

  if (item.items && item.items.length > 0) {
    return (
      <Collapsible asChild defaultOpen={item.items.some(child => pathname.startsWith(child.url || '____'))} className='group/collapsible'>
        <SidebarMenuItem>
          <CollapsibleTrigger asChild>
            <SidebarMenuButton tooltip={item.title} className='text-base' style={{ fontFamily: 'Arial', fontWeight: 200 }}>
              {item.icon && <Icon name={item.icon} />}
              <span>{item.title}</span>
              <IconChevronRight className='ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-90' />
            </SidebarMenuButton>
          </CollapsibleTrigger>
          <CollapsibleContent>
            <SidebarMenuSub>
              {item.items.map(subItem => (
                <SidebarNavItem key={subItem.title} item={subItem} />
              ))}
            </SidebarMenuSub>
          </CollapsibleContent>
        </SidebarMenuItem>
      </Collapsible>
    );
  }

  return (
    <SidebarMenuItem key={item.title}>
      <SidebarMenuButton asChild tooltip={item.title} isActive={pathname === item.url} className='text-base' style={{ fontFamily: 'Arial', fontWeight: 200 }}>
        <Link href={item.url || '#'}>
          {item.icon && <Icon name={item.icon} />}
          <span>{item.title}</span>
        </Link>
      </SidebarMenuButton>
    </SidebarMenuItem>
  );
}
