import { NavItem } from '@/types';

export const navItems: NavItem[] = [
  {
    title: 'Dashboard',
    url: '/',
    icon: 'dashboard',
  },
  {
    title: 'Members',
    url: '#',
    icon: 'user',
    items: [
      { title: 'Member List', url: '/members/list' },
      {
        title: 'Segments / Groups',
        url: '/members/segments',
      },
      { title: 'Tags Management', url: '/members/tags' },
    ]
  },
  {
    title: 'Wallets',
    url: '/wallets',
    icon: 'wallet',
  },
  {
    title: 'Tiers',
    url: '/tiers',
    icon: 'award',
  },
  {
    title: 'Campaigns',
    url: '/campaigns',
    icon: 'campaign',
  },
  {
    title: 'Loyalty Engine',
    url: '#',
    icon: 'settings',
    items: [
      { title: 'Earning Rules', url: '/loyalty/earning-rules' },
    ]
  },
  {
    title: 'Rewards',
    url: '/rewards',
    icon: 'gift',
  },
  {
    title: 'Achievements',
    url: '/achievements',
    icon: 'trophy',
  },
  {
    title: 'Coupons',
    url: '/coupons',
    icon: 'coupon',
  },
  {
    title: 'Communication',
    url: '#',
    icon: 'mail',
    items: [
      { title: 'Campaigns', url: '/communication/campaigns' },
      { title: 'Templates', url: '/communication/templates' },
      { title: 'Logs & Delivery', url: '/communication/logs' },
    ]
  },
  {
    title: 'Reports & Analytics',
    url: '/reports',
    icon: 'chart'
  },
  {
    title: 'System Settings',
    url: '/settings',
    icon: 'settings',
  },
  {
    title: 'Temp Profile',
    url: '/temp-profile',
    icon: 'user',
  },
];
