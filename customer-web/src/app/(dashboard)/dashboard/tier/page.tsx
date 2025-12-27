import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Progress } from "@/components/ui/progress"
import { Badge } from "@/components/ui/badge"
import { Award, CheckCircle2, Star, Zap } from "lucide-react"

const tierBenefits = [
    { title: 'Points Multiplier', description: 'Earn 1.5x points on every purchase', icon: Zap },
    { title: 'Free Shipping', description: 'Complimentary shipping on all orders', icon: CheckCircle2 },
    { title: 'Exclusive Access', description: 'Early access to sales and new products', icon: Star },
]

export default function TierPage() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Tier Status</h1>
            </div>

            <div className="grid gap-6 md:grid-cols-2">
                <Card className="border-amber-500/50 bg-amber-500/5 overflow-hidden relative">
                    <div className="absolute top-0 right-0 p-4 opacity-10">
                        <Award className="h-32 w-32" />
                    </div>
                    <CardHeader>
                        <div className="flex items-center gap-2">
                            <Badge variant="outline" className="bg-amber-500 text-white border-none px-3 py-1 text-sm font-bold animate-pulse">
                                GOLD MEMBER
                            </Badge>
                        </div>
                        <CardTitle className="text-2xl mt-4">Level Up in Progress</CardTitle>
                        <CardDescription className="text-foreground/70">
                            You are 750 points away from becoming a **Platinum member**.
                        </CardDescription>
                    </CardHeader>
                    <CardContent className="space-y-4">
                        <div className="flex justify-between text-sm font-medium">
                            <span>2,500 / 3,250 points</span>
                            <span>75%</span>
                        </div>
                        <Progress value={75} className="h-3 bg-amber-200" />
                        <p className="text-xs text-muted-foreground italic">
                            *Membership expires on Dec 31, 2025
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader>
                        <CardTitle>Your Gold Benefits</CardTitle>
                        <CardDescription>Exclusive perks for your current tier.</CardDescription>
                    </CardHeader>
                    <CardContent className="grid gap-4">
                        {tierBenefits.map((benefit, i) => (
                            <div key={i} className="flex items-start gap-4 p-3 rounded-lg bg-muted/30 border">
                                <div className="p-2 rounded-full bg-primary/10 text-primary">
                                    <benefit.icon className="h-5 w-5" />
                                </div>
                                <div>
                                    <h3 className="font-semibold">{benefit.title}</h3>
                                    <p className="text-sm text-muted-foreground">{benefit.description}</p>
                                </div>
                            </div>
                        ))}
                    </CardContent>
                </Card>
            </div>
        </div>
    )
}
