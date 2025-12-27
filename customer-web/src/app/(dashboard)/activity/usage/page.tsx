"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Zap, History, AlertCircle, Calendar } from "lucide-react"
import { useMemberHistory } from "@/hooks/use-member-history"

export default function CampaignUsagePage() {
    const { usage, loading, error } = useMemberHistory()

    if (loading) {
        return (
            <div className="flex h-[400px] items-center justify-center">
                <div className="text-center">
                    <History className="mx-auto h-8 w-8 animate-spin text-muted-foreground" />
                    <p className="mt-2 text-sm text-muted-foreground">Loading campaign usage...</p>
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
                <h1 className="text-3xl font-bold tracking-tight">Campaign History</h1>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Your Campaign Usage</CardTitle>
                    <CardDescription>See which campaigns you&apos;ve participated in and how many times.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Campaign</TableHead>
                                <TableHead className="text-center">Usage Count</TableHead>
                                <TableHead className="text-right">Last Used</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {usage.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={3} className="text-center py-10 text-muted-foreground">
                                        No campaign activity recorded yet.
                                    </TableCell>
                                </TableRow>
                            ) : (
                                usage.map((item) => (
                                    <TableRow key={item.id}>
                                        <TableCell>
                                            <div className="flex items-center gap-2">
                                                <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-amber-100/50 text-amber-600">
                                                    <Zap className="h-4 w-4" />
                                                </div>
                                                <span className="font-bold">{item.campaignName}</span>
                                            </div>
                                        </TableCell>
                                        <TableCell className="text-center">
                                            <Badge variant="outline" className="font-mono">
                                                {item.usageCount} times
                                            </Badge>
                                        </TableCell>
                                        <TableCell className="text-right text-muted-foreground text-sm italic">
                                            <div className="flex items-center justify-end gap-1">
                                                <Calendar className="h-3 w-3" />
                                                {item.lastUsedAt ? new Date(item.lastUsedAt).toLocaleDateString() : "Never"}
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
    )
}
