"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { PlusCircle, ShoppingBag, UserPlus, Gift, Trash2, Send, RotateCcw } from "lucide-react";
import { useToast } from "@/hooks/use-toast";

interface TransactionItem {
    sku: string;
    qty: number;
    price: number;
}

export default function PosSimulatorPage() {
    const { toast } = useToast();
    const [form, setForm] = useState({
        customerId: "CUST-1001",
        storeId: "STORE-01",
        sellerId: "EMP-220",
        currency: "INR"
    });

    const [items, setItems] = useState<TransactionItem[]>([
        { sku: "SKU-101", qty: 2, price: 400 },
        { sku: "SKU-555", qty: 1, price: 1700 }
    ]);

    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState<any>(null);
    const [members, setMembers] = useState<any[]>([]);

    useEffect(() => {
        fetch('/api/members')
            .then(res => res.json())
            .then(data => setMembers(Array.isArray(data) ? data : []))
            .catch(() => { });
    }, []);

    const handleFormChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleItemChange = (index: number, field: keyof TransactionItem, value: any) => {
        const newItems = [...items];
        (newItems[index] as any)[field] = value;
        setItems(newItems);
    };

    const addItem = () => setItems([...items, { sku: "", qty: 1, price: 0 }]);
    const removeItem = (index: number) => setItems(items.filter((_, i) => i !== index));

    const totalAmount = items.reduce((sum, item) => sum + (Number(item.qty) * Number(item.price)), 0);

    const sendTransaction = async () => {
        setLoading(true);
        setResult(null);

        const payload = {
            transaction_id: `TXN-${Date.now()}`,
            user_id: form.customerId,
            gross_value: totalAmount,
            store_id: form.storeId,
            seller_id: form.sellerId,
            currency: form.currency,
            items: items.map(i => ({ ...i, qty: Number(i.qty), price: Number(i.price) })),
            timestamp: new Date().toISOString(),
            channel: "POS"
        };

        try {
            const res = await fetch("/api/pos/transaction", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (!res.ok) throw new Error(`API Error: ${res.statusText}`);

            const data = await res.json();
            setResult(data);
            toast({
                title: "Transaction Sent",
                description: `ID: ${payload.transaction_id}`,
            });
        } catch (err: any) {
            setResult({ error: err.message });
            toast({
                title: "Failed",
                description: err.message,
                variant: "destructive"
            });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex flex-col gap-6 p-8 max-w-5xl mx-auto w-full">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold tracking-tight text-neutral-900">POS Simulator</h1>
                    <p className="text-neutral-500 mt-1">Simulate detailed customer transactions from a Point of Sale system.</p>
                </div>
                <Badge variant="outline" className="px-3 py-1 text-sm bg-green-50 text-green-700 border-green-200">
                    <span className="w-2 h-2 rounded-full bg-green-50 mr-2 inline-block"></span>
                    System Online
                </Badge>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Left Panel: Transaction Setup */}
                <div className="lg:col-span-2 space-y-6">
                    <Card className="border-neutral-200 shadow-sm">
                        <CardHeader>
                            <CardTitle className="text-lg flex items-center gap-2">
                                <UserPlus className="w-5 h-5 text-blue-600" />
                                Customer & Store Details
                            </CardTitle>
                        </CardHeader>
                        <CardContent className="grid grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <Label>Customer</Label>
                                <select
                                    name="customerId"
                                    value={form.customerId}
                                    onChange={(e) => setForm({ ...form, customerId: e.target.value })}
                                    className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
                                >
                                    <option value="" disabled>Select a customer...</option>
                                    {members.length > 0 ? (
                                        members.map((m: any) => (
                                            <option key={m.id} value={m.externalId || m.id}>
                                                {m.firstName} {m.lastName} ({m.email})
                                            </option>
                                        ))
                                    ) : (
                                        <option value="" disabled>Loading members...</option>
                                    )}
                                </select>
                            </div>
                            <div className="space-y-2">
                                <Label>Store ID</Label>
                                <Input name="storeId" value={form.storeId} onChange={handleFormChange} />
                            </div>
                            <div className="space-y-2">
                                <Label>Seller ID</Label>
                                <Input name="sellerId" value={form.sellerId} onChange={handleFormChange} />
                            </div>
                            <div className="space-y-2">
                                <Label>Currency</Label>
                                <Input name="currency" value={form.currency} onChange={handleFormChange} />
                            </div>
                        </CardContent>
                    </Card>

                    <Card className="border-neutral-200 shadow-sm">
                        <CardHeader className="flex flex-row items-center justify-between">
                            <CardTitle className="text-lg flex items-center gap-2">
                                <ShoppingBag className="w-5 h-5 text-purple-600" />
                                Cart Items
                            </CardTitle>
                            <Button size="sm" variant="outline" onClick={addItem}>
                                <PlusCircle className="w-4 h-4 mr-2" /> Add SKU
                            </Button>
                        </CardHeader>
                        <CardContent>
                            <Table>
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>SKU</TableHead>
                                        <TableHead className="w-[100px]">Qty</TableHead>
                                        <TableHead className="w-[120px]">Price</TableHead>
                                        <TableHead className="w-[50px]"></TableHead>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {items.map((item, index) => (
                                        <TableRow key={index}>
                                            <TableCell>
                                                <Input
                                                    value={item.sku}
                                                    onChange={(e) => handleItemChange(index, 'sku', e.target.value)}
                                                    placeholder="SKU-..."
                                                />
                                            </TableCell>
                                            <TableCell>
                                                <Input
                                                    type="number"
                                                    value={item.qty}
                                                    onChange={(e) => handleItemChange(index, 'qty', e.target.value)}
                                                />
                                            </TableCell>
                                            <TableCell>
                                                <Input
                                                    type="number"
                                                    value={item.price}
                                                    onChange={(e) => handleItemChange(index, 'price', e.target.value)}
                                                />
                                            </TableCell>
                                            <TableCell>
                                                <Button size="icon" variant="ghost" className="text-red-500 hover:text-red-700" onClick={() => removeItem(index)}>
                                                    <Trash2 className="w-4 h-4" />
                                                </Button>
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                            <div className="flex justify-end mt-4 text-lg font-bold">
                                Total: {totalAmount.toLocaleString()} {form.currency}
                            </div>
                        </CardContent>
                    </Card>
                </div>

                {/* Right Panel: Actions & Output */}
                <div className="space-y-6">
                    <Card className="border-blue-100 bg-blue-50/50 shadow-sm">
                        <CardHeader>
                            <CardTitle className="text-lg text-blue-900">Actions</CardTitle>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <Button className="w-full bg-blue-600 hover:bg-blue-700 text-lg h-12" onClick={sendTransaction} disabled={loading}>
                                {loading ? <RotateCcw className="mr-2 h-5 w-5 animate-spin" /> : <Send className="mr-2 h-5 w-5" />}
                                {loading ? "Processing..." : "Process Transaction"}
                            </Button>
                            <p className="text-xs text-blue-600/80 text-center">
                                This will identify the customer, calculate points based on active campaigns, and trigger rewards.
                            </p>
                        </CardContent>
                    </Card>

                    {result && (
                        <Card className="border-neutral-200 shadow-sm overflow-hidden">
                            <CardHeader className="bg-gray-50 border-b">
                                <CardTitle className="text-sm font-mono text-gray-500">API Response</CardTitle>
                            </CardHeader>
                            <CardContent className="p-0">
                                <pre className="p-4 text-xs font-mono text-blue-600 bg-slate-950 overflow-auto max-h-60">
                                    {JSON.stringify(result, null, 2)}
                                </pre>
                            </CardContent>
                        </Card>
                    )}
                </div>
            </div>
        </div>
    );
}
