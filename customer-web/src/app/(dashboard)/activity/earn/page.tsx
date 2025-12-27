import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { PlusCircle, ShoppingBag, UserPlus, Gift } from "lucide-react"

const earnEvents = [
    { id: 1, source: 'Purchase', details: 'Order #3492 - Electronics', points: 450, date: '2023-11-25 14:30' },
    { id: 2, source: 'Referral', details: 'Referred John Doe', points: 1000, date: '2023-11-22 09:15' },
    { id: 3, source: 'Campaign', details: 'Weekend Double Points Bonus', points: 450, date: '2023-11-25 14:30' },
    { id: 4, source: 'Engagement', details: 'Profile Completion', points: 100, date: '2023-11-20 18:45' },
]

export default function EarnEventsPage() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Earn Events</h1>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Earning History</CardTitle>
                    <CardDescription>Every point counts. Here is how you earned them.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Source</TableHead>
                                <TableHead>Details</TableHead>
                                <TableHead className="text-right">Points</TableHead>
                                <TableHead className="text-right">Date</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {earnEvents.map((event) => (
                                <TableRow key={event.id}>
                                    <TableCell>
                                        <div className="flex items-center gap-2">
                                            {event.source === 'Purchase' && <ShoppingBag className="h-4 w-4 text-blue-500" />}
                                            {event.source === 'Referral' && <UserPlus className="h-4 w-4 text-purple-500" />}
                                            {event.source === 'Campaign' && <PlusCircle className="h-4 w-4 text-orange-500" />}
                                            {event.source === 'Engagement' && <Gift className="h-4 w-4 text-pink-500" />}
                                            <span className="font-bold">{event.source}</span>
                                        </div>
                                    </TableCell>
                                    <TableCell className="text-muted-foreground">{event.details}</TableCell>
                                    <TableCell className="text-right">
                                        <Badge variant="success" className="font-bold">+{event.points}</Badge>
                                    </TableCell>
                                    <TableCell className="text-right text-sm">{event.date}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    )
}
