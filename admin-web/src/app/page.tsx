'use client';

import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { formatDistanceToNow } from 'date-fns';
import { PointsOverviewChart } from '@/components/charts/PointsOverviewChart';
import { CampaignPerformanceChart } from '@/components/charts/CampaignPerformanceChart';
import { TierDistributionChart } from '@/components/charts/TierDistributionChart';

interface ActivityLog {
    id: string;
    type: string;
    description: string;
    variant?: "default" | "secondary" | "destructive" | "outline";
    createdAt: string;
}

interface DashboardData {
    totalMembers: number;
    activeMembers: number;
    newMembersLastMonth: number;
    totalPointsIssued: number;
    totalPointsRedeemed: number;
    pointsBreakage: number;
    recentActivities: ActivityLog[];
    pointsHistory: any[];
    tierDistribution: any[];
    campaignPerformance: any[];
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

export default function DashboardPage() {
    const [data, setData] = useState<DashboardData | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchDashboardData = async () => {
            try {
                const response = await fetch('/api/dashboard');
                if (response.ok) {
                    const jsonData = await response.json();
                    setData(jsonData);
                }
            } catch (error) {
                console.error("Failed to fetch dashboard data:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchDashboardData();
        const interval = setInterval(fetchDashboardData, 10000); // Poll every 10 seconds
        return () => clearInterval(interval);
    }, []);

    if (loading && !data) {
        return <div className="p-8 text-center text-muted-foreground">Loading dashboard...</div>;
    }

    return (
        <div className="space-y-8">
            {/* KPI Section */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <StatCard title="Total Members" value={data?.totalMembers || 0} subtext={`+${data?.newMembersLastMonth || 0} this month`} />
                <StatCard title="Active Members" value={data?.activeMembers || 0} />
                <StatCard title="Points Issued" value={data?.totalPointsIssued.toLocaleString() || "0"} />
                <StatCard title="Points Redeemed" value={data?.totalPointsRedeemed.toLocaleString() || "0"} />
            </div>

            {/* Charts and Recent Activity Section */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                <div className="lg:col-span-2 space-y-8">
                    <Card>
                        <CardHeader><CardTitle>Points Issuance vs. Redemption (7 Days)</CardTitle></CardHeader>
                        <CardContent>
                            <PointsOverviewChart data={data?.pointsHistory || []} />
                        </CardContent>
                    </Card>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                        <Card>
                            <CardHeader><CardTitle>Campaign Performance</CardTitle></CardHeader>
                            <CardContent>
                                <CampaignPerformanceChart data={data?.campaignPerformance || []} />
                            </CardContent>
                        </Card>
                        <Card>
                            <CardHeader><CardTitle>Tier Distribution</CardTitle></CardHeader>
                            <CardContent>
                                <TierDistributionChart data={data?.tierDistribution || []} />
                            </CardContent>
                        </Card>
                    </div>
                </div>

                {/* Recent Activity Feed */}
                <div className="lg:col-span-1">
                    <Card className="h-full">
                        <CardHeader><CardTitle>Recent Activity</CardTitle></CardHeader>
                        <CardContent>
                            <Table>
                                <TableBody>
                                    {!data?.recentActivities?.length ? (
                                        <TableRow>
                                            <TableCell className="text-center py-8 text-muted-foreground">
                                                No recent activity.
                                            </TableCell>
                                        </TableRow>
                                    ) : (
                                        data.recentActivities.map((activity) => (
                                            <TableRow key={activity.id}>
                                                <TableCell>
                                                    <div className="font-medium">{activity.description}</div>
                                                    <div className="text-sm text-muted-foreground flex items-center gap-2 mt-1">
                                                        <Badge variant={(activity.variant as any) || 'secondary'}>{activity.type}</Badge>
                                                        <span>{formatDistanceToNow(new Date(activity.createdAt))} ago</span>
                                                    </div>
                                                </TableCell>
                                            </TableRow>
                                        ))
                                    )}
                                </TableBody>
                            </Table>
                        </CardContent>
                    </Card>
                </div>
            </div>

            {/* Campaign Analytics Section */}
            <div className="mt-8">
                <h2 className="text-2xl font-bold mb-4">Campaign Analytics</h2>
                <Tabs defaultValue="summary" className="space-y-4">
                    <TabsList>
                        <TabsTrigger value="summary">Summary</TabsTrigger>
                        <TabsTrigger value="points">Member Points</TabsTrigger>
                        <TabsTrigger value="transactions">Recent Transactions</TabsTrigger>
                    </TabsList>

                    {/* Summary Tab */}
                    <TabsContent value="summary" className="space-y-4">
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                            <Card>
                                <CardHeader>
                                    <CardTitle className="text-sm font-medium">Total Members with Points</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <div className="text-2xl font-bold">
                                        {data?.totalMembers || 0}
                                    </div>
                                    <p className="text-xs text-muted-foreground mt-1">
                                        Across all tiers
                                    </p>
                                </CardContent>
                            </Card>
                            <Card>
                                <CardHeader>
                                    <CardTitle className="text-sm font-medium">Points Breakage</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <div className="text-2xl font-bold">
                                        {data?.pointsBreakage?.toLocaleString() || 0}
                                    </div>
                                    <p className="text-xs text-muted-foreground mt-1">
                                        Issued - Redeemed
                                    </p>
                                </CardContent>
                            </Card>
                            <Card>
                                <CardHeader>
                                    <CardTitle className="text-sm font-medium">Redemption Rate</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <div className="text-2xl font-bold">
                                        {(data?.totalPointsIssued ?? 0) > 0
                                            ? Math.round(((data?.totalPointsRedeemed ?? 0) / (data?.totalPointsIssued ?? 1)) * 100)
                                            : 0}%
                                    </div>
                                    <p className="text-xs text-muted-foreground mt-1">
                                        Of total points issued
                                    </p>
                                </CardContent>
                            </Card>
                        </div>
                    </TabsContent>

                    {/* Member Points Tab */}
                    <TabsContent value="points" className="space-y-4">
                        <Card>
                            <CardHeader>
                                <CardTitle>Top Members by Points</CardTitle>
                                <CardDescription>
                                    Members with highest lifetime points (sample data - connect to real API)
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Rank</TableHead>
                                            <TableHead>Member ID</TableHead>
                                            <TableHead>Lifetime Points</TableHead>
                                            <TableHead>Current Balance</TableHead>
                                            <TableHead>Tier</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        <TableRow>
                                            <TableCell colSpan={5} className="text-center py-8 text-muted-foreground">
                                                Connect to /api/campaign-analytics/member-points to display real data
                                            </TableCell>
                                        </TableRow>
                                    </TableBody>
                                </Table>
                            </CardContent>
                        </Card>
                    </TabsContent>

                    {/* Transactions Tab */}
                    <TabsContent value="transactions" className="space-y-4">
                        <Card>
                            <CardHeader>
                                <CardTitle>Recent Points Transactions</CardTitle>
                                <CardDescription>
                                    Latest points earned and redeemed (sample data - connect to real API)
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Member ID</TableHead>
                                            <TableHead>Type</TableHead>
                                            <TableHead>Points</TableHead>
                                            <TableHead>Campaign</TableHead>
                                            <TableHead>Date</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        <TableRow>
                                            <TableCell colSpan={5} className="text-center py-8 text-muted-foreground">
                                                Connect to /api/campaign-analytics/transactions to display real data
                                            </TableCell>
                                        </TableRow>
                                    </TableBody>
                                </Table>
                            </CardContent>
                        </Card>
                    </TabsContent>
                </Tabs>
            </div>
        </div>
    );
}

