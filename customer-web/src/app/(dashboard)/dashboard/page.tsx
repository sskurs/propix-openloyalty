"use client"

import * as React from "react"
import { CreditCard, DollarSign, Activity, Users, Star, Gift, ArrowRight } from "lucide-react"
import Link from "next/link"

import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
    CardFooter,
} from "@/components/ui/card"
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Progress } from "@/components/ui/progress"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/hooks/use-auth"

interface DashboardTimelineItem {
    id: string;
    title: string;
    description: string;
    eventType: string;
    metadata?: {
        points?: number;
        pointsValue?: number;
    };
}

export default function DashboardPage() {
    const { member, loading } = useAuth()
    const [timeline, setTimeline] = React.useState<DashboardTimelineItem[]>([])
    const [timelineLoading, setTimelineLoading] = React.useState(true)
    const [campaigns, setCampaigns] = React.useState<any[]>([])
    const [rewards, setRewards] = React.useState<any[]>([])

    React.useEffect(() => {
        if (member?.id) {
            setTimelineLoading(true)
            fetch(`/api/customer/transactions?memberId=${member.id}`)
                .then(res => res.json())
                .then(data => {
                    setTimeline(Array.isArray(data) ? data.slice(0, 5) : [])
                    setTimelineLoading(false)
                })
                .catch(() => setTimelineLoading(false))

            fetch('/api/customer/campaigns')
                .then(res => res.json())
                .then(data => setCampaigns(Array.isArray(data) ? data.slice(0, 2) : []))
                .catch(() => { })

            fetch('/api/customer/rewards')
                .then(res => res.json())
                .then(data => setRewards(Array.isArray(data) ? data : []))
                .catch(() => { })
        }
    }, [member])

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="flex flex-col items-center gap-2">
                    <Activity className="h-8 w-8 animate-spin text-primary" />
                    <p className="text-muted-foreground animate-pulse">Loading your dashboard...</p>
                </div>
            </div>
        )
    }

    const pointsWallet = member?.wallets?.find((w) => w.type === 'points')
    const cashbackWallet = member?.wallets?.find((w) => w.type === 'cashback')

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h2 className="text-3xl font-bold tracking-tight">Welcome back, {member?.firstName || "Member"}!</h2>
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                <Card className="relative overflow-hidden">
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Points Balance</CardTitle>
                        <Star className="h-4 w-4 text-primary fill-primary/20" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">{pointsWallet?.balance || 0} pts</div>
                        <p className="text-xs text-muted-foreground">Available to redeem rewards</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Tier Status</CardTitle>
                        <Users className="h-4 w-4 text-muted-foreground" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold text-primary capitalize">{member?.tierName || "Member"}</div>
                        <div className="mt-2 space-y-1">
                            <div className="flex items-center justify-between text-[10px] text-muted-foreground uppercase font-semibold">
                                <span>Progress to next tier</span>
                                <span>40%</span>
                            </div>
                            <Progress value={40} className="h-1" />
                        </div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Cashback</CardTitle>
                        <DollarSign className="h-4 w-4 text-muted-foreground" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">${cashbackWallet?.balance?.toFixed(2) || "0.00"}</div>
                        <p className="text-xs text-muted-foreground">Saved from your purchases</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Rewards</CardTitle>
                        <Gift className="h-4 w-4 text-muted-foreground" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold">+{rewards.length}</div>
                        <p className="text-xs text-muted-foreground">Exclusive offers available</p>
                    </CardContent>
                </Card>
            </div>

            <div className="grid gap-4 md:grid-cols-7">
                <Card className="md:col-span-4">
                    <CardHeader>
                        <CardTitle>Recent Activity</CardTitle>
                        <CardDescription>Your latest earning and redemption events.</CardDescription>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Event</TableHead>
                                    <TableHead className="text-right">Value</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {timelineLoading ? (
                                    <TableRow>
                                        <TableCell colSpan={2} className="text-center py-4">Loading activity...</TableCell>
                                    </TableRow>
                                ) : timeline.length === 0 ? (
                                    <TableRow>
                                        <TableCell colSpan={2} className="text-center py-4 text-muted-foreground">No recent activity.</TableCell>
                                    </TableRow>
                                ) : (
                                    timeline.map((item) => (
                                        <TableRow key={item.id}>
                                            <TableCell>
                                                <div className="font-medium text-sm">{item.title}</div>
                                                <div className="text-[10px] text-muted-foreground line-clamp-1">{item.description}</div>
                                            </TableCell>
                                            <TableCell className="text-right">
                                                <Badge variant={item.eventType === 'POINT_EARN' ? 'secondary' : 'outline'}>
                                                    {item.eventType === 'POINT_EARN' ? '+' : '-'}{item.metadata?.points || item.metadata?.pointsValue || 0}
                                                </Badge>
                                            </TableCell>
                                        </TableRow>
                                    ))
                                )}
                            </TableBody>
                        </Table>
                    </CardContent>
                    <CardFooter>
                        <Button variant="ghost" className="w-full text-muted-foreground hover:text-primary" asChild>
                            <Link href="/history">
                                View all activity <ArrowRight className="ml-2 h-4 w-4" />
                            </Link>
                        </Button>
                    </CardFooter>
                </Card>

                <div className="md:col-span-3 space-y-4">
                    <Card>
                        <CardHeader>
                            <CardTitle>Active Campaigns</CardTitle>
                            <CardDescription>Don&apos;t miss out on extra points!</CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            {campaigns.length > 0 ? (
                                campaigns.map(c => (
                                    <div key={c.id} className="flex items-center justify-between gap-4 border-b border-neutral-50 pb-2 last:border-0 last:pb-0">
                                        <div className="space-y-1">
                                            <p className="text-sm font-medium leading-none">{c.name}</p>
                                            <p className="text-[10px] text-muted-foreground uppercase">
                                                {c.validTo ? `Ends ${new Date(c.validTo).toLocaleDateString()}` : "Active"}
                                            </p>
                                        </div>
                                        <Button size="sm" variant="ghost" className="h-7 text-xs" asChild><Link href="/earn">View</Link></Button>
                                    </div>
                                ))
                            ) : (
                                <p className="text-sm text-center text-muted-foreground py-4">No active campaigns.</p>
                            )}
                        </CardContent>
                    </Card>

                    <Card className="bg-primary text-primary-foreground shadow-lg border-none overflow-hidden relative">
                        <div className="absolute -bottom-4 -right-4 p-4 opacity-10 pointer-events-none">
                            <Gift className="h-32 w-32" />
                        </div>
                        <CardHeader>
                            <CardTitle className="text-white">Featured Reward</CardTitle>
                            <CardDescription className="text-primary-foreground/70">
                                {rewards[0] ? `Redeem for ${rewards[0].costPoints} points!` : "Check out our points rewards!"}
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <p className="text-lg font-bold truncate">
                                {rewards[0]?.name || "Exclusive Rewards"}
                            </p>
                        </CardContent>
                        <CardFooter>
                            <Button className="w-full bg-white text-primary hover:bg-neutral-100 border-none font-semibold" asChild>
                                <Link href="/rewards">Browse All</Link>
                            </Button>
                        </CardFooter>
                    </Card>
                </div>
            </div>
        </div>
    )
}
