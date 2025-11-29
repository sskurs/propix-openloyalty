'use client';

import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useState } from 'react';
import { IconPlus, IconTrash } from '@tabler/icons-react';

const RuleBuilder = () => {
    return (
        <div className="p-4 border rounded-md bg-slate-50 dark:bg-slate-800/50">
            <div className="flex items-center space-x-2 mb-4">
                <Label>IF</Label>
                <Select defaultValue="and">
                    <SelectTrigger className="w-auto"><SelectValue /></SelectTrigger>
                    <SelectContent><SelectItem value="and">All (AND)</SelectItem><SelectItem value="or">Any (OR)</SelectItem></SelectContent>
                </Select>
                <Label>of the following conditions are met:</Label>
            </div>
            <div className="space-y-4 pl-8">
                <div className="flex items-center space-x-2">
                   <Select><SelectTrigger className="w-1/4"><SelectValue placeholder="Context" /></SelectTrigger><SelectContent><SelectItem value="Member">Member</SelectItem><SelectItem value="Transaction">Transaction</SelectItem></SelectContent></Select>
                   <Select><SelectTrigger className="w-1/4"><SelectValue placeholder="Field" /></SelectTrigger><SelectContent><SelectItem value="Tier">Tier</SelectItem><SelectItem value="Amount">Amount</SelectItem></SelectContent></Select>
                   <Select><SelectTrigger className="w-auto"><SelectValue placeholder="Operator" /></SelectTrigger><SelectContent><SelectItem value="in">in</SelectItem><SelectItem value="gte">{'>='}</SelectItem></SelectContent></Select>
                   <Input placeholder="Value" className="w-1/3"/>
                   <Button variant="ghost" size="icon"><IconTrash className="h-4 w-4 text-red-500" /></Button>
                </div>
                 <div className="pt-2"><Button variant="outline" size="sm"><IconPlus className="h-4 w-4 mr-2" />Add Condition</Button></div>
            </div>
        </div>
    )
}

export default function CreateEarningRulePage() {
    const [step, setStep] = useState(1);
    const [category, setCategory] = useState('BASE');
    const [formulaType, setFormulaType] = useState('RATE_PER_AMOUNT');

    return (
        <div>
            <h1 className="text-3xl font-bold mb-8">Create Earning Rule</h1>
            
            {/* For simplicity, a single-page form is used here. A multi-step wizard can be added later. */}
            <div className="space-y-6 max-w-3xl mx-auto">
                 <Card>
                    <CardHeader><CardTitle>Basics</CardTitle><CardDescription>Define the rule's purpose and trigger.</CardDescription></CardHeader>
                    <CardContent className="space-y-4">
                        <div className="space-y-2"><Label>Rule Name</Label><Input placeholder="e.g., Base Spend Rule" /></div>
                        <div className="space-y-2"><Label>Category</Label><Select value={category} onValueChange={setCategory}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value="BASE">Base</SelectItem><SelectItem value="BOOSTER">Booster</SelectItem><SelectItem value="EVENT">Event</SelectItem></SelectContent></Select></div>
                        <div className="space-y-2"><Label>Description</Label><Textarea placeholder="Internal note about this rule." /></div>
                        <div className="space-y-2"><Label>Trigger Event</Label><Select><SelectTrigger><SelectValue placeholder="Select an event" /></SelectTrigger><SelectContent><SelectItem value="PURCHASE_COMPLETED">PURCHASE_COMPLETED</SelectItem><SelectItem value="MEMBER_REGISTERED">MEMBER_REGISTERED</SelectItem></SelectContent></Select></div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader><CardTitle>Conditions</CardTitle><CardDescription>Define when this rule should apply.</CardDescription></CardHeader>
                    <CardContent><RuleBuilder /></CardContent>
                </Card>

                <Card>
                    <CardHeader><CardTitle>Earning Formula</CardTitle><CardDescription>Define how many points are awarded.</CardDescription></CardHeader>
                    <CardContent className="space-y-4">
                        <div className="space-y-2"><Label>Formula Type</Label><Select value={formulaType} onValueChange={setFormulaType}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value="RATE_PER_AMOUNT">Rate per amount</SelectItem><SelectItem value="FIXED">Fixed points</SelectItem></SelectContent></Select></div>
                        {formulaType === 'RATE_PER_AMOUNT' && <div className="flex items-center gap-2">Give <Input type="number" defaultValue={1} className="w-20" /> point(s) for every <Input type="number" defaultValue={10} className="w-20" /> currency unit(s).</div>}
                        {formulaType === 'FIXED' && <div className="flex items-center gap-2">Give <Input type="number" defaultValue={500} className="w-24" /> point(s) per event.</div>}
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader><CardTitle>Validity & Priority</CardTitle></CardHeader>
                    <CardContent className="space-y-4">
                        <div className="grid grid-cols-2 gap-4">
                            <div className="space-y-2"><Label>Valid From</Label><Input type="datetime-local" /></div>
                            <div className="space-y-2"><Label>Valid Until (Optional)</Label><Input type="datetime-local" /></div>
                        </div>
                        <div className="space-y-2"><Label>Priority</Label><Input type="number" defaultValue={100} /></div>
                    </CardContent>
                </Card>

                <div className="flex justify-end gap-2 mt-8">
                    <Button variant="secondary">Save as Draft</Button>
                    <Button>Save & Activate</Button>
                </div>
            </div>
        </div>
    )
}
