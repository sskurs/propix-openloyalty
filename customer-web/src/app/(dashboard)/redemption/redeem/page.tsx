import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Gift, ArrowRight, ShieldCheck, Ticket, AlertCircle } from "lucide-react"

export default function RedeemPage() {
    return (
        <div className="max-w-3xl mx-auto space-y-6 py-10">
            <div className="text-center space-y-2">
                <h1 className="text-3xl font-extrabold tracking-tight">Confirm Redemption</h1>
                <p className="text-muted-foreground italic">You are one step away from your reward!</p>
            </div>

            <div className="grid gap-6 md:grid-cols-3">
                <Card className="md:col-span-2">
                    <CardHeader>
                        <CardTitle>Selected Reward</CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-6">
                        <div className="flex items-center gap-6 p-4 rounded-xl bg-muted/20 border-2 border-dashed border-muted">
                            <div className="h-20 w-20 rounded-full bg-primary/10 flex items-center justify-center text-5xl">
                                💰
                            </div>
                            <div>
                                <Badge className="mb-1">Store Credit</Badge>
                                <h2 className="text-xl font-bold">$10 Store Credit</h2>
                                <p className="text-sm text-muted-foreground">Expires in 30 days after redemption.</p>
                            </div>
                        </div>

                        <div className="space-y-4">
                            <div className="flex justify-between items-center text-sm">
                                <span className="text-muted-foreground">Original Price:</span>
                                <span className="font-medium">1,200 Points</span>
                            </div>
                            <div className="flex justify-between items-center text-sm text-green-600 font-bold">
                                <span>Campaign Discount:</span>
                                <span>-200 Points</span>
                            </div>
                            <div className="border-t pt-4 flex justify-between items-center font-bold text-lg">
                                <span>Total to Pay:</span>
                                <span className="text-primary">1,000 Points</span>
                            </div>
                        </div>
                    </CardContent>
                    <CardFooter className="flex-col gap-3">
                        <Button className="w-full h-12 text-lg font-bold shadow-lg shadow-primary/20">
                            Confirm & Redeem <ArrowRight className="ml-2 h-5 w-5" />
                        </Button>
                        <p className="text-[10px] text-center text-muted-foreground px-10">
                            By clicking &quot;Confirm &amp; Redeem&quot;, you agree to the Terms of Service. This action cannot be undone.
                        </p>
                    </CardFooter>
                </Card>

                <div className="space-y-6">
                    <Card className="bg-primary/5">
                        <CardHeader className="pb-2">
                            <CardTitle className="text-sm">Available Balance</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="text-3xl font-bold">2,450</div>
                            <p className="text-xs text-muted-foreground mt-1 text-green-600 font-medium">Insufficient? Earn more</p>
                        </CardContent>
                    </Card>

                    <div className="p-4 rounded-lg bg-amber-50 border border-amber-200 text-amber-800 text-xs space-y-2">
                        <div className="flex items-center gap-2 font-bold uppercase">
                            <AlertCircle className="h-4 w-4" />
                            Important Note
                        </div>
                        <p>Digital vouchers will be sent to your registered email immediately.</p>
                    </div>

                    <div className="flex items-center gap-2 justify-center text-muted-foreground">
                        <ShieldCheck className="h-4 w-4" />
                        <span className="text-[10px] font-bold uppercase tracking-widest text-foreground">Secure Redemption</span>
                    </div>
                </div>
            </div>
        </div>
    )
}
