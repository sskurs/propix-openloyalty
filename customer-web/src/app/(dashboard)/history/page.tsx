"use client"

import * as React from "react"
import { History, ArrowUpRight, ArrowDownLeft, TrendingUp } from "lucide-react"

import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
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
import { useAuth } from "@/hooks/use-auth"
interface HistoryTimelineItem {
    id: string;
    title: string;
    description: string;
    eventType: string;
    occurredAt: string;
    metadata?: {
        points?: number;
        pointsValue?: number;
    };
}

export default function HistoryPage() {
    const { member, loading } = useAuth()
    const [history, setHistory] = React.useState<HistoryTimelineItem[]>([])
    const [historyLoading, setHistoryLoading] = React.useState(true)

    React.useEffect(() => {
        if (member?.id) {
            fetch(`/api/customer/transactions?memberId=${member.id}`)
                .then(res => res.json())
                .then(data => {
                    setHistory(data)
                    setHistoryLoading(false)
                })
                .catch(() => setHistoryLoading(false))
        }
    }, [member])

    if (loading) {
        return <div className="p-10 text-center">Loading history...</div>
    }

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Activity History</h2>
                    <p className="text-muted-foreground">
                        A detailed log of your points earned, rewards redeemed, and tier changes.
                    </p>
                </div>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Recent Events</CardTitle>
                    <CardDescription>
                        View your loyalty journey in chronological order.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead className="w-[100px]">Type</TableHead>
                                <TableHead>Description</TableHead>
                                <TableHead>Date</TableHead>
                                <TableHead className="text-right">Amount</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {historyLoading ? (
                                <TableRow>
                                    <TableCell colSpan={4} className="text-center py-10">
                                        Loading activity logs...
                                    </TableCell>
                                </TableRow>
                            ) : history.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={4} className="text-center py-10">
                                        No activity recorded yet.
                                    </TableCell>
                                </TableRow>
                            ) : (
                                history.map((item) => (
                                    <TableRow key={item.id}>
                                        <TableCell>
                                            {item.eventType === 'POINT_EARN' && (
                                                <Badge variant="outline" className="text-green-600 border-green-200 bg-green-50">
                                                    <ArrowUpRight className="mr-1 h-3 w-3" /> Earn
                                                </Badge>
                                            )}
                                            {item.eventType === 'REWARD_REDEEM' && (
                                                <Badge variant="outline" className="text-blue-600 border-blue-200 bg-blue-50">
                                                    <ArrowDownLeft className="mr-1 h-3 w-3" /> Redeem
                                                </Badge>
                                            )}
                                            {item.eventType === 'TIER_CHANGE' && (
                                                <Badge variant="outline" className="text-amber-600 border-amber-200 bg-amber-50">
                                                    <TrendingUp className="mr-1 h-3 w-3" /> Tier
                                                </Badge>
                                            )}
                                        </TableCell>
                                        <TableCell>
                                            <div className="font-medium">{item.title}</div>
                                            <div className="text-sm text-muted-foreground">
                                                {item.description}
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-muted-foreground italic">
                                            {new Date(item.occurredAt).toLocaleDateString()}
                                        </TableCell>
                                        <TableCell className={`text-right font-bold ${item.eventType === 'POINT_EARN' ? 'text-green-600' : 'text-red-600'}`}>
                                            {item.eventType === 'POINT_EARN' ? '+' : '-'}{item.metadata?.points || item.metadata?.pointsValue || 0}
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    )
}
