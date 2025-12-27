"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { CheckCircle, Clock, XCircle, AlertCircle } from "lucide-react"
import { useMemberHistory } from "@/hooks/use-member-history"

export default function RedemptionHistoryPage() {
    const { redemptions, loading, error } = useMemberHistory()

    if (loading) {
        return (
            <div className="flex h-[400px] items-center justify-center">
                <div className="text-center">
                    <History className="mx-auto h-8 w-8 animate-spin text-muted-foreground" />
                    <p className="mt-2 text-sm text-muted-foreground">Loading redemptions...</p>
                </div>
            </div>
        )
    }

    if (error) {
        return (
            <div className="flex h-[400px] items-center justify-center">
                <div className="text-center text-destructive">
                    <AlertCircle className="mx-auto h-8 w-8" />
                    <p className="mt-2 font-medium">Error loading history</p>
                    <p className="text-sm opacity-80">{error}</p>
                </div>
            </div>
        )
    }

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Redemption History</h1>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Your Recent Redemptions</CardTitle>
                    <CardDescription>Track the status of your redeemed rewards.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Order ID</TableHead>
                                <TableHead>Reward</TableHead>
                                <TableHead>Cost</TableHead>
                                <TableHead>Status</TableHead>
                                <TableHead className="text-right">Date</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {redemptions.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={5} className="text-center py-10 text-muted-foreground">
                                        No redemptions found.
                                    </TableCell>
                                </TableRow>
                            ) : (
                                redemptions.map((item) => (
                                    <TableRow key={item.id}>
                                        <TableCell className="font-mono text-[10px] opacity-70">
                                            {item.id.split('-')[0]}...
                                        </TableCell>
                                        <TableCell className="font-medium">
                                            {item.rewardName}
                                            {item.quantity > 1 && <span className="ml-2 text-xs opacity-60">x{item.quantity}</span>}
                                        </TableCell>
                                        <TableCell>
                                            <div className="flex flex-col">
                                                {item.pointsSpent && item.pointsSpent > 0 && (
                                                    <span className="font-bold text-amber-600">{item.pointsSpent} pts</span>
                                                )}
                                                {item.cashbackSpent && item.cashbackSpent > 0 && (
                                                    <span className="font-bold text-emerald-600">${item.cashbackSpent}</span>
                                                )}
                                                {(!item.pointsSpent && !item.cashbackSpent) && (
                                                    <span className="text-muted-foreground italic">Free</span>
                                                )}
                                            </div>
                                        </TableCell>
                                        <TableCell>
                                            <div className="flex items-center gap-2">
                                                {item.status.toLowerCase() === 'completed' && <CheckCircle className="h-3 w-3 text-green-500" />}
                                                {item.status.toLowerCase() === 'pending' && <Clock className="h-3 w-3 text-amber-500" />}
                                                {item.status.toLowerCase() === 'rejected' && <XCircle className="h-3 w-3 text-red-500" />}
                                                <Badge variant={
                                                    item.status.toLowerCase() === 'completed' ? 'default' :
                                                        item.status.toLowerCase() === 'pending' ? 'secondary' : 'destructive'
                                                } className="capitalize text-[10px]">
                                                    {item.status}
                                                </Badge>
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-right text-muted-foreground text-xs">
                                            {new Date(item.createdAt).toLocaleDateString()}
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

import { History } from "lucide-react"
