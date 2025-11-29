'use client';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { useState } from 'react';
import { Badge } from '@/components/ui/badge';

const ruleFields = {
    RFM: ['recency_days', 'frequency', 'monetary'],
    Profile: ['age', 'gender', 'country', 'city'],
    Loyalty: ['tier', 'points_balance'],
};

const RuleRow = ({ index, rule, onRemove, onUpdate }: { index: number; rule: any; onRemove: (index: number) => void; onUpdate: (index: number, rule: any) => void; }) => {
    const showPeriodInput = ['frequency', 'monetary'].includes(rule.field);

    const handleFieldChange = (key: string, value: any) => {
        onUpdate(index, { ...rule, [key]: value });
    };

    return (
        <div className="flex items-center space-x-2 p-3 rounded-lg bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700">
            <Select value={rule.field} onValueChange={(value) => handleFieldChange('field', value)}>
                <SelectTrigger className="w-48"><SelectValue placeholder="Select a field..." /></SelectTrigger>
                <SelectContent>
                    {Object.entries(ruleFields).map(([group, options]) => (
                        <div key={group}>
                            <Label className="px-2 py-1.5 text-xs font-semibold text-muted-foreground">{group}</Label>
                            {options.map(opt => <SelectItem value={opt} key={opt}>{opt.replace('_', ' ')}</SelectItem>)}
                        </div>
                    ))}
                </SelectContent>
            </Select>

            <Select value={rule.operator} onValueChange={(value) => handleFieldChange('operator', value)}>
                <SelectTrigger className="w-32"><SelectValue placeholder="Operator" /></SelectTrigger>
                <SelectContent>
                    <SelectItem value="gte">&gt;= (is at least)</SelectItem>
                    <SelectItem value="lte">&lt;= (is at most)</SelectItem>
                    <SelectItem value="eq">= (is equal to)</SelectItem>
                    <SelectItem value="neq">!= (is not)</SelectItem>
                </SelectContent>
            </Select>

            <Input placeholder="Value" className="w-32" value={rule.value} onChange={(e) => handleFieldChange('value', e.target.value)} />

            {showPeriodInput && (
                <div className="flex items-center gap-2 whitespace-nowrap text-sm">
                    <span>in the last</span>
                    <Input placeholder="e.g., 365" className="w-24" type="number" value={rule.period_days || ''} onChange={(e) => handleFieldChange('period_days', parseInt(e.target.value, 10) || null)} />
                    <span>days</span>
                </div>
            )}

            <Button variant="ghost" size="icon" onClick={() => onRemove(index)}>
                <IconTrash className="h-4 w-4 text-red-500" />
            </Button>
        </div>
    );
};

const RuleBuilder = () => {
    const [logic, setLogic] = useState('and');
    const [rules, setRules] = useState([
        { field: 'recency_days', operator: 'lte', value: '30' },
        { field: 'frequency', operator: 'gte', value: '4', period_days: 365 },
        { field: 'monetary', operator: 'gte', value: '1000', period_days: 365 },
    ]);

    const addRule = () => {
        setRules([...rules, { field: '', operator: '', value: '' }]);
    };

    const removeRule = (index: number) => {
        setRules(rules.filter((_, i) => i !== index));
    };

    const updateRule = (index: number, newRule: any) => {
        const newRules = [...rules];
        newRules[index] = newRule;
        setRules(newRules);
    };

    return (
        <div className="p-4 border rounded-md bg-slate-50 dark:bg-slate-800/50 mt-6">
            <div className="flex items-center space-x-2 mb-4">
                <Label className="font-semibold text-lg">Conditions</Label>
                <Select value={logic} onValueChange={setLogic}>
                    <SelectTrigger className="w-auto"><SelectValue /></SelectTrigger>
                    <SelectContent>
                        <SelectItem value="and">Match ALL (AND)</SelectItem>
                        <SelectItem value="or">Match ANY (OR)</SelectItem>
                    </SelectContent>
                </Select>
            </div>

            <div className="space-y-3 pl-4">
                {rules.map((rule, index) => (
                    <RuleRow key={index} index={index} rule={rule} onRemove={removeRule} onUpdate={updateRule} />
                ))}
                 <div className="pt-2">
                    <Button variant="outline" size="sm" type="button" onClick={addRule}>
                        <IconPlus className="h-4 w-4 mr-2" />Add Condition
                    </Button>
                </div>
            </div>
        </div>
    )
}

export default function CreateDynamicSegmentPage() {
  return (
    <div>
      <h1 className="text-3xl font-bold mb-4">Create Dynamic Segment</h1>
      <p className="text-muted-foreground mb-8">Define rules based on member behavior and attributes to create a dynamic segment.</p>
      <form>
        <Card className="max-w-5xl mx-auto">
          <CardHeader>
            <CardTitle>Segment Details</CardTitle>
          </CardHeader>
          <CardContent>
              <div className="space-y-4">
                  <div className="space-y-2">
                      <Label htmlFor="name">Segment Name</Label>
                      <Input id="name" placeholder="e.g., Champions" defaultValue="Champions" required />
                  </div>
                  <div className="space-y-2">
                      <Label htmlFor="description">Description</Label>
                      <Textarea id="description" placeholder="Members with high recency, frequency, and monetary value." defaultValue="Recency ≤ 30 & Frequency ≥ 4 & Monetary ≥ 1000"/>
                  </div>
                  <RuleBuilder />
              </div>
          </CardContent>
          <CardFooter className="flex justify-end gap-2">
              <Button type="button" variant="secondary">Cancel</Button>
              <Button type="submit">Create & Activate</Button>
          </CardFooter>
        </Card>
      </form>
    </div>
  );
}
