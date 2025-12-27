import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Gift, Search, SlidersHorizontal, ShoppingCart, Info } from "lucide-react"

const rewards = [
    { id: 1, name: '$10 Store Credit', points: 1000, category: 'Credit', img: '💰' },
    { id: 2, name: 'Free Delivery Voucher', points: 500, category: 'Voucher', img: '🚚' },
    { id: 3, name: 'Exclusive Mug', points: 1500, category: 'Merchandise', img: '☕' },
    { id: 4, name: '$50 Electronics Discount', points: 4000, category: 'Discount', img: '💻' },
    { id: 5, name: 'VIP Event Invite', points: 10000, category: 'Experience', img: '🎟️' },
    { id: 6, name: 'Mystery Gift Box', points: 5000, category: 'Surprise', img: '🎁' },
]

export default function RewardsCatalogPage() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Rewards Catalog</h1>
                <div className="flex items-center gap-2">
                    <Button variant="outline" size="sm" className="hidden md:flex">
                        <SlidersHorizontal className="h-4 w-4 mr-2" /> Filter
                    </Button>
                    <Button variant="outline" size="sm" className="hidden md:flex">
                        <Search className="h-4 w-4 mr-2" /> Search
                    </Button>
                </div>
            </div>

            <div className="grid gap-6 grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
                {rewards.map((reward) => (
                    <Card key={reward.id} className="overflow-hidden hover:shadow-md transition-all group border-muted-foreground/10">
                        <div className="aspect-square bg-muted/30 flex items-center justify-center text-6xl group-hover:scale-110 transition-transform">
                            {reward.img}
                        </div>
                        <CardHeader className="p-4">
                            <div className="flex justify-between items-start">
                                <Badge variant="secondary" className="mb-2 text-[10px] uppercase font-bold tracking-tighter px-1.5">{reward.category}</Badge>
                            </div>
                            <CardTitle className="text-base font-bold">{reward.name}</CardTitle>
                        </CardHeader>
                        <CardContent className="p-4 pt-0">
                            <div className="flex items-baseline gap-1">
                                <span className="text-xl font-black text-amber-500">{reward.points}</span>
                                <span className="text-[10px] text-muted-foreground uppercase font-bold">Points</span>
                            </div>
                        </CardContent>
                        <CardFooter className="p-4 pt-0">
                            <Button variant="default" className="w-full text-xs h-8">
                                <ShoppingCart className="h-3 w-3 mr-1.5" /> Redeem
                            </Button>
                        </CardFooter>
                    </Card>
                ))}
            </div>
        </div>
    )
}
