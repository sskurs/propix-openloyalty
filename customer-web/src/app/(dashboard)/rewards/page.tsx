"use client"

import * as React from "react"
import { Gift, Wallet, CheckCircle2 } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { toast } from "sonner"
import { useAuth } from "@/hooks/use-auth"

interface Reward {
    id: string;
    name: string;
    description?: string;
    pointsCost?: number;
}

export default function RewardsPage() {
    const { member } = useAuth()
    const [rewards, setRewards] = React.useState<Reward[]>([])
    const [loading, setLoading] = React.useState(true)
    const [isRedeeming, setIsRedeeming] = React.useState<string | null>(null)

    React.useEffect(() => {
        fetch("/api/customer/rewards")
            .then((res) => res.json())
            .then((data) => {
                setRewards(data)
                setLoading(false)
            })
            .catch(() => setLoading(false))
    }, [])

    const handleRedeem = (reward: Reward) => {
        const pointsWallet = member?.wallets?.find((w: { type: string, balance: number }) => w.type === 'points')
        if ((pointsWallet?.balance || 0) < (reward.pointsCost || 100)) {
            toast.error("Insufficient points balance!")
            return
        }

        setIsRedeeming(reward.id)
        // Simulate API call
        setTimeout(() => {
            setIsRedeeming(null)
            toast.success(`Successfully redeemed: ${reward.name}`, {
                description: "A voucher code has been sent to your email.",
                icon: <CheckCircle2 className="text-green-500 h-5 w-5" />
            })
        }, 1500)
    }

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="flex flex-col items-center gap-2">
                    <Gift className="h-8 w-8 animate-bounce text-primary" />
                    <p className="text-muted-foreground animate-pulse">Loading rewards catalog...</p>
                </div>
            </div>
        )
    }

    return (
        <div className="space-y-6 pb-20">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Rewards</h2>
                    <p className="text-muted-foreground">
                        Exclusive perks and vouchers just for you.
                    </p>
                </div>
                <div className="flex items-center gap-2 bg-muted/50 px-4 py-2 rounded-lg border">
                    <Wallet className="h-4 w-4 text-primary" />
                    <span className="text-sm font-semibold">Your Balance: {member?.wallets?.find((w: { type: string, balance: number }) => w.type === 'points')?.balance || 0} pts</span>
                </div>
            </div>

            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
                {rewards.length === 0 ? (
                    <div className="col-span-full border-2 border-dashed rounded-xl p-20 text-center space-y-2">
                        <Gift className="h-10 w-10 text-muted-foreground/50 mx-auto" />
                        <h3 className="text-xl font-semibold">No rewards available</h3>
                        <p className="text-muted-foreground">Check back soon for new offers.</p>
                    </div>
                ) : (
                    rewards.map((reward) => (
                        <Card key={reward.id} className="flex flex-col group overflow-hidden border-2 hover:border-primary/50 transition-all">
                            <div className="aspect-video w-full bg-muted/30 relative flex items-center justify-center group-hover:bg-muted/50 transition-colors">
                                <Gift className="h-12 w-12 text-muted-foreground/30 group-hover:scale-110 transition-transform" />
                                <div className="absolute top-2 right-2">
                                    <Badge variant="default" className="shadow-lg">{reward.pointsCost || 100} pts</Badge>
                                </div>
                            </div>
                            <CardHeader className="p-4">
                                <CardTitle className="text-lg line-clamp-1 group-hover:text-primary transition-colors">{reward.name}</CardTitle>
                                <CardDescription className="line-clamp-2 min-h-[2.5rem]">
                                    {reward.description || "Get this exclusive reward today!"}
                                </CardDescription>
                            </CardHeader>
                            <CardFooter className="p-4 pt-0 mt-auto">
                                <Button
                                    className="w-full shadow-sm"
                                    onClick={() => handleRedeem(reward)}
                                    disabled={isRedeeming !== null}
                                >
                                    {isRedeeming === reward.id ? "Processing..." : "Redeem Now"}
                                </Button>
                            </CardFooter>
                        </Card>
                    ))
                )}
            </div>
        </div>
    )
}
