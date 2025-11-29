'use client';

import { useEffect, useState, use, Fragment } from 'react';
import { Customer as MemberDetails, Wallet, TimelineEvent, ExpiringPoint, WalletTransaction, Tier, TierHistoryItem, Transaction, CustomEvent, Campaign, Achievement, MemberReward } from '@/types';
import { Card, CardContent, CardHeader, CardTitle ,CardDescription} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import { ArrowLeft, GitCommit, Heart, Minus, Plus, Star, Tag, TrendingUp, User as UserIcon } from 'lucide-react';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { getMemberDetails, getMemberWallets } from '@/api/membersApi';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Progress } from "@/components/ui/progress";
import { format, isToday, isYesterday } from 'date-fns';
import { Label } from '@/components/ui/label';

// Mock Data based on your specification
const mockData: {
  points: Wallet;
  cashback: Wallet;
  transactions: WalletTransaction[];
  expiringPoints: ExpiringPoint[];
  timelineEvents: TimelineEvent[];
  currentTier: Tier;
  tierHistory: TierHistoryItem[];
  commerceTransactions: Transaction[];
  customEvents: CustomEvent[];
  campaigns: Campaign[];
  achievements: Achievement[];
  rewards: { active: MemberReward[]; history: MemberReward[] };
} = {
  points: { id: 'points-wallet', type: 'POINTS', balance: 6500, expiringSoon: 500, nextExpiryDate: '2025-03-31' },
  cashback: { id: 'cashback-wallet', type: 'CASHBACK', balance: 120.00, currency: 'INR' },
  transactions: [
    { id: 'txn-1', date: "2025-02-05T10:32:00Z", walletType: "POINTS", direction: "EARN", amount: 200, balanceAfter: 6500, reason: "Purchase", referenceType: "ORDER", referenceId: "ORD-1234", campaignId: 42 },
    { id: 'txn-2', date: "2025-02-04T16:09:00Z", walletType: "POINTS", direction: "SPEND", amount: 100, balanceAfter: 6300, reason: "Reward redemption", referenceType: "REWARD", referenceId: "REW-101" },
  ],
  expiringPoints: [
    { expiryDate: "2025-03-31", points: 200, source: "Campaign: Diwali Booster" },
    { expiryDate: "2025-04-15", points: 300, source: "Base earn" }
  ],
  timelineEvents: [
    { id: 1001, eventType: "POINT_EARN", title: "Points earned", description: "+200 pts from Order #1234 (Campaign: Weekend Booster)", walletType: "POINTS", pointsChange: 200, balanceAfter: 6500, referenceType: "ORDER", referenceId: "ORD-1234", createdAt: new Date().toISOString() },
    { id: 1000, eventType: "TIER_CHANGE", title: "Tier changed", description: "Tier changed from SILVER to GOLD", oldValue: 'SILVER', newValue: 'GOLD', referenceType: "TIER", referenceId: "GOLD", createdAt: new Date(new Date().setDate(new Date().getDate() - 1)).toISOString() },
  ],
  currentTier: {
    id: 'gold-tier',
    name: 'GOLD',
    level: 3,
    pointsThreshold: 5000,
    since: '2024-01-10',
    nextTier: 'PLATINUM',
    progress: { current: 3750, required: 5000 },
  },
  tierHistory: [
    { tierName: 'GOLD', startDate: '2024-01-10', endDate: undefined },
    { tierName: 'SILVER', startDate: '2023-06-01', endDate: '2024-01-09' },
  ],
  commerceTransactions: [
      { id: 'ORD-1234', date: '2025-02-05T10:32:00Z', amount: 2000, currency: 'INR', items: [{name: 'Item A', quantity: 1}] },
      { id: 'ORD-1201', date: '2025-02-01T09:00:00Z', amount: 1200, currency: 'INR', items: [{name: 'Item B', quantity: 2}] },
  ],
  customEvents: [
      { id: 'evt-1', name: 'APP_LOGIN', date: '2025-02-03T12:21:00Z', data: { device: 'Android', appVersion: '3.2.1' } },
  ],
  campaigns: [
      { id: 'camp-42', name: 'Weekend Booster', description: 'Get double points on weekends' },
  ],
  achievements: [
      { id: 'ach-1', name: 'Weekend Shopper', description: 'Made 3 purchases on weekends' },
      { id: 'ach-2', name: 'Big Spender', description: 'Spent over ₹20,000' },
  ],
  rewards: {
      active: [{ id: 'rew-101', rewardName: 'Free Coffee', status: 'ISSUED', issueDate: '2025-02-04', expiryDate: '2025-03-31' }],
      history: [{ id: 'rew-80', rewardName: '10% Off Coupon', status: 'EXPIRED', issueDate: '2024-11-01', expiryDate: '2024-12-15' }]
  }
};


// Helper component for displaying a detail item
function DetailItem({ label, value, isTruncated = false }: { label: string; value: React.ReactNode; isTruncated?: boolean }) {
    return (
        <div>
            <p className="text-sm text-muted-foreground">{label}</p>
            <p className={`text-sm font-semibold ${isTruncated ? 'truncate' : ''}`}>{value || '-'}</p>
        </div>
    );
}

function StatCard({ title, value, subtext }: { title: string, value: string | number, subtext?: string }) {
    return (
        <Card>
            <CardHeader className="pb-2">
                <p className="text-sm text-muted-foreground">{title}</p>
            </CardHeader>
            <CardContent>
                <p className="text-3xl font-bold">{value}</p>
                {subtext && <p className="text-xs text-muted-foreground">{subtext}</p>}
            </CardContent>
        </Card>
    );
}

// --- Timeline Components ---
const eventIconMap: Record<TimelineEvent['eventType'], React.ReactElement> = {
    POINT_EARN: <Plus className="h-4 w-4" />,
    POINT_SPEND: <Minus className="h-4 w-4" />,
    POINT_EXPIRE: <Minus className="h-4 w-4" />,
    CASHBACK_EARN: <Plus className="h-4 w-4" />,
    CASHBACK_SPEND: <Minus className="h-4 w-4" />,
    TIER_CHANGE: <TrendingUp className="h-4 w-4" />,
    PROFILE_UPDATE: <UserIcon className="h-4 w-4" />,
    TAG_ADDED: <Tag className="h-4 w-4" />,
    TAG_REMOVED: <Tag className="h-4 w-4" />,
    REWARD_REDEEM: <Star className="h-4 w-4" />,
    CAMPAIGN_ENROLLED: <Heart className="h-4 w-4" />,
    LOGIN_EVENT: <GitCommit className="h-4 w-4" />,
};

const groupEventsByDate = (events: TimelineEvent[]) => {
    return events.reduce((acc, event) => {
        const date = new Date(event.createdAt);
        let groupLabel;
        if (isToday(date)) {
            groupLabel = 'Today';
        } else if (isYesterday(date)) {
            groupLabel = 'Yesterday';
        } else {
            groupLabel = format(date, 'MMMM d, yyyy');
        }
        if (!acc[groupLabel]) {
            acc[groupLabel] = [];
        }
        acc[groupLabel].push(event);
        return acc;
    }, {} as Record<string, TimelineEvent[]>);
};

const TimelineEventItem = ({ event }: { event: TimelineEvent }) => (
    <div className="flex items-start gap-4">
        <div className="flex h-8 w-8 items-center justify-center rounded-full bg-muted">
            {eventIconMap[event.eventType]}
        </div>
        <div className="flex-1 space-y-1">
            <p className='text-sm font-medium leading-none'>{event.description || event.title}</p>
            <p className='text-sm text-muted-foreground'>{format(new Date(event.createdAt), 'h:mm a')}</p>
            {event.balanceAfter != null && <p className='text-xs text-muted-foreground'>Balance: {event.balanceAfter} pts</p>}
        </div>
    </div>
);
// --- End Timeline Components ---

function MemberProfilePage() {
    const pointsWallet = mockData.points;
    const cashbackWallet = mockData.cashback;
    const groupedTimelineEvents = groupEventsByDate(mockData.timelineEvents);
    const currentTier = mockData.currentTier;
    const tierHistory = mockData.tierHistory;
    const commerceTransactions = mockData.commerceTransactions;
    const customEvents = mockData.customEvents;
    const campaigns = mockData.campaigns;
    const achievements = mockData.achievements;
    const rewards = mockData.rewards;
    const member = { firstName: 'John', lastName: 'Doe', email: 'john.doe@example.com', referralCode: 'REF123', id: '12345', phoneNumber: '555-555-5555', loyaltyCardNumber: 'LOYAL123', loyaltyTierName: 'GOLD', isActive: true }; // mock member

    return (
        <div className="space-y-4">
            <div className="flex flex-col md:flex-row gap-4 w-full">
                <main className="flex-1">
                    <Tabs defaultValue="overview">
                        <TabsList>
                            <TabsTrigger value="general-info">General Info</TabsTrigger>
                            <TabsTrigger value="dashboard">Dashboard</TabsTrigger>
                            <TabsTrigger value="wallets">Wallets</TabsTrigger>
                            <TabsTrigger value="timeline">Timeline</TabsTrigger>
                            <TabsTrigger value="tiers">Tiers</TabsTrigger>
                            <TabsTrigger value="transactions">Transactions</TabsTrigger>
                            <TabsTrigger value="events">Custom Events</TabsTrigger>
                            <TabsTrigger value="campaigns">Campaigns</TabsTrigger>
                            <TabsTrigger value="achievements">Achievements</TabsTrigger>
                            <TabsTrigger value="rewards">Rewards</TabsTrigger>
                        </TabsList>
                        <TabsContent value="general-info" className="py-4">
                                                <Card>
                                                    <CardHeader>
                                                        <CardTitle>Member Information</CardTitle>
                                                        <CardDescription>Personal and loyalty-related details for this member.</CardDescription>
                                                    </CardHeader>
                                                    <CardContent className="space-y-6">
                                                        <div className="flex items-center space-x-4">
                                                            <Avatar className="h-24 w-24">
                                                                <AvatarImage src={`https://api.dicebear.com/7.x/initials/svg?seed=${member.firstName} ${member.lastName}`} />
                                                                <AvatarFallback>{member.firstName?.[0]}{member.lastName?.[0]}</AvatarFallback>
                                                            </Avatar>
                                                            <div className="flex-1">
                                                                <h2 className="text-2xl font-bold">{member.firstName} {member.lastName}</h2>
                                                                <Badge variant={member.isActive ? 'success' : 'destructive'} className="mt-1">
                                                                    {member.isActive ? 'Active' : 'Inactive'}
                                                                </Badge>
                                                            </div>
                                                        </div>
                                                        <Separator />
                                                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                                                            <div className="space-y-1"><Label>Email</Label><p className="text-sm font-medium">{member.email}</p></div>
                                                            <div className="space-y-1"><Label>Phone Number</Label><p className="text-sm font-medium">{member.phoneNumber || 'N/A'}</p></div>
                                                            <div className="space-y-1"><Label>Gender</Label><p className="text-sm font-medium">{member.gender || 'N/A'}</p></div>
                                                            <div className="space-y-1"><Label>Date of Birth</Label><p className="text-sm font-medium">{member.dateOfBirth ? format(parseISO(member.dateOfBirth), 'PPP') : 'N/A'}</p></div>
                                                            <div className="space-y-1"><Label>Member ID</Label><p className="text-sm font-medium text-muted-foreground truncate">{member.id}</p></div>
                                                            <div className="space-y-1"><Label>Joined On</Label><p className="text-sm font-medium">{member.registrationDate ? format(parseISO(member.registrationDate), 'PPP') : 'N/A'}</p></div>
                                                            <div className="space-y-1"><Label>Current Tier</Label><p className="text-sm font-medium">{member.loyaltyTierName}</p></div>
                                                            <div className="space-y-1"><Label>Loyalty Card No</Label><p className="text-sm font-medium">{member.loyaltyCardNumber || 'N/A'}</p></div>
                                                            <div className="space-y-1"><Label>Referral Code</Label><p className="text-sm font-medium">{member.referralCode || 'N/A'}</p></div>
                                                        </div>
                                                    </CardContent>
                                                </Card>
                                            </TabsContent>
                        <TabsContent value="dashboard" className="py-4">
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                                <StatCard title="Total Spending" value="$4,823.50" />
                                <StatCard title="Purchase Transactions" value="72" />
                                <StatCard title="Avg. Purchase" value="$67.00" />
                                <StatCard title="Total Returns" value="$150.00" />
                                <StatCard title="Return Transactions" value="2" />
                                <StatCard title="Avg. Returns" value="$75.00" />
                                <StatCard title="Days Since Last Transaction" value="14" />
                                <StatCard title="Predicted Spending (Next 30d)" value="$250" />
                                <StatCard title="Probability of Next Purchase" value="82%" />
                            </div>
                        </TabsContent>
                        <TabsContent value="wallets" className="py-4">
                            <div className="space-y-8">
                                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                                <StatCard title="Points" value={pointsWallet.balance.toLocaleString()} subtext="Total points available" />
                                <StatCard title="Cashback" value={`${cashbackWallet.balance.toFixed(2)} ${cashbackWallet.currency}`} subtext="Available cashback" />
                                <StatCard title="Expiring Points" value={`${pointsWallet.expiringSoon} pts`} subtext={`Expiring in next 30 days`} />
                                </div>

                                <Card>
                                <CardHeader><CardTitle>Expiring Points</CardTitle></CardHeader>
                                <CardContent>
                                    <Table>
                                        <TableHeader><TableRow><TableHead>Expiry Date</TableHead><TableHead>Points</TableHead><TableHead>Source</TableHead></TableRow></TableHeader>
                                        <TableBody>
                                            {mockData.expiringPoints.map((p, i) => (
                                                <TableRow key={i}><TableCell>{p.expiryDate}</TableCell><TableCell>{p.points}</TableCell><TableCell>{p.source}</TableCell></TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                                </Card>

                                <Card>
                                <CardHeader><CardTitle>Wallet Ledger</CardTitle></CardHeader>
                                <CardContent>
                                    <Table>
                                        <TableHeader>
                                            <TableRow>
                                                <TableHead>Date/Time</TableHead>
                                                <TableHead>Wallet Type</TableHead>
                                                <TableHead>Direction</TableHead>
                                                <TableHead>Amount</TableHead>
                                                <TableHead>Balance After</TableHead>
                                                <TableHead>Reason</TableHead>
                                                <TableHead>Reference</TableHead>
                                            </TableRow>
                                        </TableHeader>
                                        <TableBody>
                                            {mockData.transactions.map(tx => (
                                                <TableRow key={tx.id}>
                                                    <TableCell>{new Date(tx.date).toLocaleString()}</TableCell>
                                                    <TableCell><Badge variant="secondary">{tx.walletType}</Badge></TableCell>
                                                    <TableCell><Badge variant={tx.direction === 'EARN' ? 'default' : 'destructive'}>{tx.direction}</Badge></TableCell>
                                                    <TableCell>{tx.direction === 'EARN' ? '+' : '-'}{tx.amount}</TableCell>
                                                    <TableCell>{tx.balanceAfter}</TableCell>
                                                    <TableCell>{tx.reason}</TableCell>
                                                    <TableCell>{tx.referenceType}: {tx.referenceId}</TableCell>
                                                </TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                                </Card>
                            </div>
                        </TabsContent>
                        <TabsContent value="timeline" className="py-4">
                           <Card>
                                <CardHeader><CardTitle>Timeline</CardTitle></CardHeader>
                                <CardContent className="space-y-6">
                                    {Object.entries(groupedTimelineEvents).map(([group, events]) => (
                                        <Fragment key={group}>
                                            <h3 className="text-lg font-semibold">{group}</h3>
                                            <div className="relative space-y-6 pl-8 before:absolute before:inset-y-0 before:left-3.5 before:w-px before:bg-muted">
                                                {events.map(event => <TimelineEventItem key={event.id} event={event} />)}
                                            </div>
                                        </Fragment>
                                    ))}
                                </CardContent>
                            </Card>
                        </TabsContent>
                        <TabsContent value="tiers" className="py-4">
                            <div className="space-y-8">
                                <Card>
                                    <CardHeader><CardTitle>Current Tier</CardTitle></CardHeader>
                                    <CardContent className="space-y-4">
                                        <div className="flex items-center gap-4">
                                            <h3 className="text-2xl font-bold">{currentTier.name} ⭐</h3>
                                            <p className="text-muted-foreground">Since: {currentTier.since}</p>
                                        </div>
                                        {currentTier.progress && currentTier.nextTier &&
                                            <div>
                                                <p className="text-sm text-muted-foreground">Need {currentTier.progress.required - currentTier.progress.current} more points by 2025-12-31 to reach {currentTier.nextTier}</p>
                                                <Progress value={(currentTier.progress.current / currentTier.progress.required) * 100} className="mt-2" />
                                                <p className="text-sm font-semibold mt-1">{currentTier.progress.current} / {currentTier.progress.required} pts</p>
                                            </div>
                                        }
                                    </CardContent>
                                </Card>
                                <Card>
                                    <CardHeader><CardTitle>Tier History</CardTitle></CardHeader>
                                    <CardContent>
                                        <Table>
                                            <TableHeader><TableRow><TableHead>Date</TableHead><TableHead>Event</TableHead><TableHead>Details</TableHead></TableRow></TableHeader>
                                            <TableBody>
                                                {tierHistory.map((item, i) => (
                                                    <TableRow key={i}><TableCell>{item.startDate}</TableCell><TableCell>{item.tierName}</TableCell><TableCell>{item.endDate}</TableCell></TableRow>
                                                ))}
                                            </TableBody>
                                        </Table>
                                    </CardContent>
                                </Card>
                            </div>
                        </TabsContent>
                        <TabsContent value="transactions" className="py-4">
                            <Card>
                                <CardHeader><CardTitle>Commerce Transactions</CardTitle></CardHeader>
                                <CardContent>
                                    <Table>
                                        <TableHeader><TableRow><TableHead>Order Date</TableHead><TableHead>Order ID</TableHead><TableHead>Channel</TableHead><TableHead>Store</TableHead><TableHead>Amount</TableHead><TableHead>Points Earned</TableHead><TableHead>Cashback Earned</TableHead></TableRow></TableHeader>
                                        <TableBody>
                                            {commerceTransactions.map((tx, i) => (
                                                <TableRow key={i}><TableCell>{new Date(tx.date).toLocaleString()}</TableCell><TableCell>{tx.id}</TableCell><TableCell>'N/A'</TableCell><TableCell>'N/A'</TableCell><TableCell>{tx.amount} {tx.currency}</TableCell><TableCell>'N/A'</TableCell><TableCell>'N/A'</TableCell></TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                            </Card>
                        </TabsContent>
                        <TabsContent value="events" className="py-4">
                            <Card>
                                <CardHeader><CardTitle>Custom Events</CardTitle></CardHeader>
                                <CardContent>
                                    <Table>
                                        <TableHeader><TableRow><TableHead>Date/Time</TableHead><TableHead>Event Type</TableHead><TableHead>Payload</TableHead></TableRow></TableHeader>
                                        <TableBody>
                                            {customEvents.map((event, i) => (
                                                <TableRow key={i}><TableCell>{new Date(event.date).toLocaleString()}</TableCell><TableCell>{event.name}</TableCell><TableCell>{JSON.stringify(event.data)}</TableCell></TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                            </Card>
                        </TabsContent>
                        <TabsContent value="campaigns" className="py-4">
                            <Card>
                                <CardHeader><CardTitle>Campaigns</CardTitle></CardHeader>
                                <CardContent>
                                    <Table>
                                        <TableHeader><TableRow><TableHead>Campaign Name</TableHead><TableHead>Status</TableHead><TableHead>Member Progress</TableHead><TableHead>Validity</TableHead></TableRow></TableHeader>
                                        <TableBody>
                                            {campaigns.map((campaign, i) => (
                                                <TableRow key={i}><TableCell>{campaign.name}</TableCell><TableCell><Badge variant={'default'}>ACTIVE</Badge></TableCell><TableCell>1 uses (34 pts)</TableCell><TableCell>'N/A'</TableCell></TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                            </Card>
                        </TabsContent>
                        <TabsContent value="achievements" className="py-4">
                            <Card>
                                <CardHeader><CardTitle>Achievements</CardTitle></CardHeader>
                                <CardContent>
                                    <Table>
                                        <TableHeader><TableRow><TableHead>Achievement</TableHead><TableHead>Status</TableHead><TableHead>Progress</TableHead><TableHead>Completed</TableHead></TableRow></TableHeader>
                                        <TableBody>
                                            {achievements.map((achievement, i) => (
                                                <TableRow key={i}><TableCell>{achievement.name}</TableCell><TableCell><Badge variant={'default'}>IN_PROGRESS</Badge></TableCell><TableCell>'N/A'</TableCell><TableCell>'N/A'</TableCell></TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                            </Card>
                        </TabsContent>
                        <TabsContent value="rewards" className="py-4">
                            <div className="space-y-8">
                                <Card>
                                    <CardHeader><CardTitle>Active Rewards</CardTitle></CardHeader>
                                    <CardContent>
                                        <Table>
                                            <TableHeader><TableRow><TableHead>Reward</TableHead><TableHead>Code</TableHead><TableHead>Cost</TableHead><TableHead>Expires</TableHead></TableRow></TableHeader>
                                            <TableBody>
                                                {rewards.active.map((reward, i) => (
                                                    <TableRow key={i}><TableCell>{reward.rewardName}</TableCell><TableCell>'N/A'</TableCell><TableCell>'N/A'</TableCell><TableCell>{format(new Date(reward.expiryDate!), 'dd MMM yyyy')}</TableCell></TableRow>
                                                ))}
                                            </TableBody>
                                        </Table>
                                    </CardContent>
                                </Card>
                                <Card>
                                    <CardHeader><CardTitle>Reward History</CardTitle></CardHeader>
                                    <CardContent>
                                        <Table>
                                            <TableHeader><TableRow><TableHead>Date</TableHead><TableHead>Reward</TableHead><TableHead>Status</TableHead><TableHead>Details</TableHead></TableRow></TableHeader>
                                            <TableBody>
                                                {rewards.history.map((reward, i) => (
                                                    <TableRow key={i}><TableCell>{format(new Date(reward.issueDate!), 'dd MMM yyyy')}</TableCell><TableCell>{reward.rewardName}</TableCell><TableCell><Badge variant={reward.status === 'REDEEMED' ? 'default' : 'destructive'}>{reward.status}</Badge></TableCell><TableCell>'N/A'</TableCell></TableRow>
                                            ))}
                                        </TableBody>
                                    </Table>
                                </CardContent>
                                </Card>
                            </div>
                        </TabsContent>
                    </Tabs>
                </main>
            </div>
        </div>
    );
}

export default MemberProfilePage;
