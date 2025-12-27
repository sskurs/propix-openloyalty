"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Gift, CreditCard, Ticket, History, AlertCircle } from "lucide-react"
import { useMemberHistory } from "@/hooks/use-member-history"

export default function ActivityRedemptionsPage() {
    const { redemptions, loading, error } = useMemberHistory()

    if (loading) {
        return (
            <div className="flex h-[400px] items-center justify-center">
                <div className="text-center">
                    <History className="mx-auto h-8 w-8 animate-spin text-muted-foreground" />
                    <p className="mt-2 text-sm text-muted-foreground">Loading redemption history...</p>
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
                <h1 className="text-3xl font-bold tracking-tight">Redemption Events</h1>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Spending History</CardTitle>
                    <CardDescription>View all your point redemptions in one place.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Reward</TableHead>
                                <TableHead className="text-right">Cost</TableHead>
                                <TableHead className="text-right">Date</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {redemptions.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={3} className="text-center py-10 text-muted-foreground">
                                        No redemptions found.
                                    </TableCell>
                                </TableRow>
                            ) : (
                                redemptions.map((red) => (
                                    <TableRow key={red.id}>
                                        <TableCell>
                                            <div className="flex items-center gap-2">
                                                <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-rose-100/50 text-rose-600">
                                                    <Ticket className="h-4 w-4" />
                                                </div>
                                                <div className="flex flex-col">
                                                    <span className="font-bold">{red.rewardName}</span>
                                                    <span className="text-[10px] text-muted-foreground uppercase">{red.status}</span>
                                                </div>
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-right">
                                            <div className="flex flex-col items-end">
                                                {red.pointsSpent && red.pointsSpent > 0 && (
                                                    <Badge variant="destructive" className="font-bold text-[10px]">-{red.pointsSpent} pts</Badge>
                                                )}
                                                {red.cashbackSpent && red.cashbackSpent > 0 && (
                                                    <Badge variant="destructive" className="font-bold text-[10px]">-${red.cashbackSpent}</Badge>
                                                )}
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-right text-muted-foreground text-sm">
                                            {new Date(red.createdAt).toLocaleDateString()}
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
