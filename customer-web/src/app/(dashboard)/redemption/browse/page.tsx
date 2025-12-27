import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Gift, Search, SlidersHorizontal, ShoppingCart, Info, ExternalLink } from "lucide-react"
import Link from "next/link"

const rewards = [
    { id: 1, name: '$10 Store Credit', points: 1000, category: 'Credit', img: '💰', available: true },
    { id: 2, name: 'Free Delivery Voucher', points: 500, category: 'Voucher', img: '🚚', available: true },
    { id: 3, name: 'Exclusive Mug', points: 1500, category: 'Merchandise', img: '☕', available: false },
    { id: 4, name: '$50 Electronics Discount', points: 4000, category: 'Discount', img: '💻', available: true },
    { id: 5, name: 'VIP Event Invite', points: 10000, category: 'Experience', img: '🎟️', available: true },
    { id: 6, name: 'Mystery Gift Box', points: 5000, category: 'Surprise', img: '🎁', available: true },
]

export default function BrowseRewardsPage() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Browse Rewards</h1>
                <Link href="/redemption/history">
                    <Button variant="link" size="sm">
                        View History <ExternalLink className="h-3 w-3 ml-1" />
                    </Button>
                </Link>
            </div>

            <div className="grid gap-6 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
                {rewards.map((reward) => (
                    <Card key={reward.id} className={`overflow-hidden flex flex-row h-32 hover:border-primary/50 transition-colors ${!reward.available ? 'opacity-60 grayscale' : ''}`}>
                        <div className="w-32 bg-muted/30 flex items-center justify-center text-4xl">
                            {reward.img}
                        </div>
                        <div className="flex-1 flex flex-col p-4 justify-between">
                            <div>
                                <Badge variant="outline" className="text-[10px] h-4 px-1">{reward.category}</Badge>
                                <CardTitle className="text-sm mt-1">{reward.name}</CardTitle>
                            </div>
                            <div className="flex items-center justify-between mt-2">
                                <span className="text-lg font-bold text-amber-500">{reward.points} <small className="text-[10px] text-muted-foreground uppercase">pts</small></span>
                                {reward.available ? (
                                    <Link href={`/redemption/redeem?id=${reward.id}`}>
                                        <Button size="sm" className="h-7 text-xs px-3">Redeem</Button>
                                    </Link>
                                ) : (
                                    <span className="text-xs text-muted-foreground font-medium uppercase">Out of Stock</span>
                                )}
                            </div>
                        </div>
                    </Card>
                ))}
            </div>
        </div>
    )
}
