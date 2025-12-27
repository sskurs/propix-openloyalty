'use client';
import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
  SelectGroup,
  SelectLabel,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { IconPlus, IconTrash, IconLoader2, IconBolt } from '@tabler/icons-react';
import { toast } from 'sonner';
import { RuleBuilder } from './rule-builder/RuleBuilder';
import { RuleGroup, createEmptyRuleGroup, convertToMicrosoftRulesEngine } from '@/types/rules';

const steps = [
  'Basics',
  'Audience',
  'Rules',
  'Rewards',
  'Schedule',
  'Review & Launch',
];

const FIELDS = {
  "Event Payload": {
    "event.payload.gross_value": "Gross Value",
    "min_order_amount": "Minimum Order Amount",
    "event.payload.category": "Category",
    "event.payload.payment_method": "Payment Method",
  },
  "Member Data": {
    "member.Tier": "Tier",
    "member.PointsBalance": "Points Balance",
  },
  "Transaction History": {
    "history_tx_count": "Total Transaction Count",
    "history_tx_sum": "Total Spend (Lifetime)"
  }
};



export interface CampaignWizardProps {
  initialData?: any;
  isEdit?: boolean;
}

export function CampaignWizard({ initialData, isEdit }: CampaignWizardProps) {
  const router = useRouter();
  const [step, setStep] = useState(1);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [formData, setFormData] = useState({
    code: initialData?.code || initialData?.Code || '',
    name: initialData?.name || initialData?.Name || '',
    description: initialData?.description || initialData?.Description || '',
    type: initialData?.type || initialData?.Type || 'points-multiplier',
    status: initialData?.status || initialData?.Status || 'ACTIVE',
    validFrom: (initialData?.startAt || initialData?.StartAt) ? new Date(initialData.startAt || initialData.StartAt).toISOString().slice(0, 16) : new Date().toISOString().slice(0, 16),
    validTo: (initialData?.endAt || initialData?.EndAt) ? new Date(initialData.endAt || initialData.EndAt).toISOString().slice(0, 16) : '',
    priority: initialData?.priority ?? initialData?.Priority ?? 0,
    maxTotalRewards: initialData?.maxTotalRewards ?? initialData?.MaxTotalRewards ?? '',
    maxPerCustomer: initialData?.maxPerCustomer ?? initialData?.MaxPerCustomer ?? '',
  });

  // Update formData when initialData changes (for edit mode)
  useEffect(() => {
    if (initialData?.type || initialData?.Type) {
      setFormData(prev => ({
        ...prev,
        type: initialData.type || initialData.Type
      }));
    }
  }, [initialData?.type, initialData?.Type]);

  // Helper to parse nested JSON safely
  const parseJson = (str: string) => {
    try {
      return JSON.parse(str);
    } catch {
      return {};
    }
  };

  // 1. Initialize Audience from Conditions
  const [audience, setAudience] = useState<any>(() => {
    const aud: { tiers: string[]; minPoints: string; maxPoints: string } = {
      tiers: [],
      minPoints: '',
      maxPoints: ''
    };
    if (initialData?.conditions) {
      initialData.conditions.forEach((c: any) => {
        const type = c.conditionType || c.ConditionType;
        const op = c.operator || c.Operator;
        const val = extractValue(c.value || c.Value);

        if (type === 'member.Tier' && op === 'in') {
          aud.tiers = Array.isArray(val) ? val : (val ? [val] : []);
        } else if (type === 'member.PointsBalance') {
          if (op === 'gte') aud.minPoints = val?.toString() || '';
          if (op === 'lte') aud.maxPoints = val?.toString() || '';
        }
      });
    }
    return aud;
  });

  const [tiers, setTiers] = useState<any[]>([]);
  // Use a refined initialization for audience if we can finding it in conditions, else default.
  // SINCE the previous submit logic ONLY mapped `rules` (Step 3) to `conditions`, 
  // and likely missed `audience` (Step 2), the Audience data might NOT be saving to DB correctly yet 
  // unless I missed where Audience is merged into conditions.
  // checking Submit logic... `conditions = rules.map(...)`. 
  // It seems I missed mapping Audience to Conditions in the Submit handler! 
  // So 'Target tiers' missing is because they aren't being saved!

  // For now, I will implement the READ logic assuming they ARE saved (or will be).
  // I will likewise fix the WRITE logic to ensure they gets saved.

  // Helper to safely extract value from JSON or raw string
  const extractValue = (val: any) => {
    if (val === null || val === undefined) return '';
    if (typeof val !== 'string') return val;

    try {
      let parsed = JSON.parse(val);

      // Handle double-serialized JSON 
      if (typeof parsed === 'string') {
        try {
          const doubleParsed = JSON.parse(parsed);
          parsed = doubleParsed;
        } catch { /* Stay as single-parsed string */ }
      }

      // If it's an object with a 'value' property, return that
      if (parsed && typeof parsed === 'object' && 'value' in parsed) {
        return parsed.value;
      }
      return parsed;
    } catch {
      return val;
    }
  };

  // Placeholder removed as it's merged above

  // Rules state (Step 3)
  const [useAdvancedRuleBuilder, setUseAdvancedRuleBuilder] = useState(() => {
    // Infer advanced mode if ruleGroupJson exists on ANY condition
    if (initialData?.conditions) {
      return initialData.conditions.some((c: any) => !!(c.ruleGroupJson || c.RuleGroupJson));
    }
    return false;
  });
  const [conditionOperator, setConditionOperator] = useState<'and' | 'or'>('and');
  const [rules, setRules] = useState(() => {
    if (initialData?.conditions) {
      const rulesFound = initialData.conditions
        .filter((c: any) => {
          const type = c.conditionType || c.ConditionType;
          return type && !type.startsWith('member.') && type !== 'complex';
        })
        .map((c: any) => ({
          field: c.conditionType || c.ConditionType,
          op: c.operator || c.Operator,
          value: extractValue(c.value || c.Value)
        }));

      if (rulesFound.length > 0) return rulesFound;
    }
    return [{ field: '', op: 'eq', value: '' }];
  });

  // Advanced Rule Builder state
  const [ruleGroup, setRuleGroup] = useState<RuleGroup>(() => {
    // Try to load from RuleEditorStateJson FIRST (new separate field)
    if (initialData?.conditions) {
      const condWithEditorState = initialData.conditions.find((c: any) => c.ruleEditorStateJson || c.RuleEditorStateJson);
      if (condWithEditorState) {
        try {
          return JSON.parse(condWithEditorState.ruleEditorStateJson || condWithEditorState.RuleEditorStateJson);
        } catch { /* Fall through */ }
      }

      // Fallback: Try RuleGroupJson (old integrated format)
      const condWithJson = initialData.conditions.find((c: any) => c.ruleGroupJson || c.RuleGroupJson);
      if (condWithJson) {
        try {
          const parsed = JSON.parse(condWithJson.ruleGroupJson || condWithJson.RuleGroupJson);
          return parsed.editorState || parsed;
        } catch { /* Fall through */ }
      }
    }
    // Return empty rule group as fallback (don't reference other state variables here)
    return createEmptyRuleGroup();
  });

  // Rewards state
  const [rewardConfig, setRewardConfig] = useState(() => {
    if (initialData?.rewards && initialData.rewards.length > 0) {
      const r = initialData.rewards[0];
      const type = r.rewardType || r.RewardType;
      let val;
      try {
        val = JSON.parse(r.value || r.Value);
      } catch {
        val = r.value || r.Value;
      }

      if (type === 'points-multiplier') {
        const factor = val?.factor ?? val;
        return { type: 'points-multiplier', value: factor?.toString() || '', points: '', amount: '' };
      } else if (type === 'point_addition' || type === 'bonus-points') {
        const points = val?.points ?? val;
        return { type: 'bonus-points', points: points?.toString() || '', value: '', amount: '' };
      } else if (type === 'cashback') {
        const percentage = val?.percentage ?? val;
        return { type: 'cashback', value: percentage?.toString() || '', points: '', amount: '' };
      } else if (type === 'COUPON') {
        const code = val?.couponCode ?? val;
        return { type: 'COUPON', value: code || '', points: '', amount: '' };
      }
    }
    return { type: initialData?.type || initialData?.Type || 'points-multiplier', value: '', points: '', amount: '' };
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { id, value } = e.target;
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const handleSelectChange = (value: string, id: string) => {
    setFormData((prev) => ({ ...prev, [id]: value }));
  };

  const addRule = () => setRules([...rules, { field: '', op: 'eq', value: '' }]);
  const removeRule = (index: number) => setRules(rules.filter((_: any, i: number) => i !== index));
  const updateRule = (index: number, field: string, value: any) => {
    const newRules = [...rules];
    (newRules[index] as any)[field] = value;
    setRules(newRules);
  };

  const nextStep = () => setStep((prev) => Math.min(prev + 1, steps.length));
  const prevStep = () => setStep((prev) => Math.max(prev - 1, 1));

  const handleSubmit = async (status: string) => {
    if (!formData.name || formData.name.length < 5) {
      toast.error('Campaign name must be at least 5 characters long');
      setStep(1);
      return;
    }

    setIsSubmitting(true);
    try {
      // Build conditions array
      const conditions = [];

      // Add Audience Tiers Condition
      if (audience.tiers.length > 0) {
        conditions.push({
          type: 'member.Tier',
          operator: 'in',
          value: JSON.stringify(audience.tiers)
        });
      }

      // Add Points Balance Conditions
      if (audience.minPoints) {
        conditions.push({
          type: 'member.PointsBalance',
          operator: 'gte',
          value: JSON.stringify({ value: parseInt(audience.minPoints) })
        });
      }
      if (audience.maxPoints) {
        conditions.push({
          type: 'member.PointsBalance',
          operator: 'lte',
          value: JSON.stringify({ value: parseInt(audience.maxPoints) })
        });
      }

      // Add Rules Engine Conditions (Step 3) - Only if not using advanced builder
      if (!useAdvancedRuleBuilder) {
        rules.forEach((r: any) => {
          if (r.field) {
            conditions.push({
              type: r.field,
              operator: r.op,
              value: JSON.stringify(r.value)
            });
          }
        });
      }

      const rewards = [];
      if (rewardConfig.type === 'points-multiplier') {
        rewards.push({
          type: 'points-multiplier',
          value: JSON.stringify({ factor: parseFloat(rewardConfig.value || '1') })
        });
      } else if (rewardConfig.type === 'bonus-points' || rewardConfig.points) {
        rewards.push({
          type: 'bonus-points',
          value: JSON.stringify({ points: parseInt(rewardConfig.points || '0') })
        });
      } else if (rewardConfig.type === 'cashback') {
        rewards.push({
          type: 'cashback',
          value: JSON.stringify({ percentage: parseFloat(rewardConfig.value || '0') })
        });
      } else if (rewardConfig.type === 'COUPON') {
        rewards.push({
          type: 'COUPON',
          value: JSON.stringify({ couponCode: rewardConfig.value })
        });
      }

      const payload = {
        code: formData.code || undefined, // API handles generation if null/empty
        name: formData.name,
        description: formData.description,
        status: status,
        type: formData.type, // Persist campaign goal/type
        startAt: new Date(formData.validFrom).toISOString(),
        endAt: formData.validTo ? new Date(formData.validTo).toISOString() : null,
        priority: parseInt(formData.priority.toString()) || 0,
        isStackable: true,
        maxTotalRewards: formData.maxTotalRewards ? parseInt(formData.maxTotalRewards.toString()) : null,
        maxPerCustomer: formData.maxPerCustomer ? parseInt(formData.maxPerCustomer.toString()) : null,
        conditions: conditions,
        ruleGroup: useAdvancedRuleBuilder ? convertToMicrosoftRulesEngine(ruleGroup, initialData?.id || "new") : null,
        ruleEditorState: useAdvancedRuleBuilder ? ruleGroup : null,
        rewards: rewards,
        rowVersion: initialData?.rowVersion || initialData?.RowVersion // Optimistic Concurrency Control
      };

      const url = isEdit ? `/api/campaigns/${initialData.id}` : '/api/campaigns';
      const method = isEdit ? 'PUT' : 'POST';

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        if (response.status === 409) {
          const errData = await response.json();
          if (errData.error === 'CAMPAIGN_CONFLICT') {
            toast.error(errData.message, {
              description: 'Please reload the page to see the latest changes.',
              duration: 5000,
            });
            setIsSubmitting(false);
            return;
          }
        }

        const error = await response.json();
        throw new Error(error.message || 'Failed to save campaign');
      }

      toast.success(`Campaign ${isEdit ? 'updated' : (status === 'ACTIVE' ? 'activated' : 'saved')} successfully`);
      router.push('/campaigns');
    } catch (error) {
      console.error(error);
      toast.error(`An error occurred while ${isEdit ? 'updating' : 'creating'} the campaign`);
    } finally {
      setIsSubmitting(false);
    }
  };

  useEffect(() => {
    fetch('/api/tiers')
      .then(res => res.json())
      .then(data => setTiers(data))
      .catch(err => console.error('Failed to fetch tiers', err));
  }, []);

  const toggleAudienceTier = (tierCode: string) => {
    setAudience((prev: any) => {
      const newTiers = prev.tiers.includes(tierCode)
        ? prev.tiers.filter((t: string) => t !== tierCode)
        : [...prev.tiers, tierCode];
      return { ...prev, tiers: newTiers };
    });
  };

  return (
    <div>
      <div className="mb-8">
        <ol className="flex items-center w-full">
          {steps.map((title, index) => (
            <li key={index} className={`flex w-full items-center ${index < steps.length - 1 ? "after:content-[''] after:w-full after:h-px after:border-b after:border-blue-100 after:border-1 after:hidden sm:after:inline-block after:mx-4 xl:after:mx-8 dark:after:border-blue-800" : ""}`}>
              <div className="flex flex-col items-center">
                <span
                  className={`flex items-center justify-center w-10 h-10 ${step >= index + 1 ? 'bg-blue-100 dark:bg-blue-800' : 'bg-gray-100 dark:bg-gray-700'} rounded-full lg:h-12 lg:w-12 shrink-0`}
                >
                  {step > index + 1 ? <svg className="w-4 h-4 text-blue-600 dark:text-blue-300" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 16 12"><path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 5.917 5.724 10.5 15 1.5" /></svg> : index + 1}
                </span>
                <span className="text-[10px] text-center mt-2 ">{title}</span>
              </div>
            </li>
          ))}
        </ol>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{steps[step - 1]}</CardTitle>
        </CardHeader>
        <CardContent className="min-h-[300px]">
          {step === 1 && (
            <div className="space-y-4 max-w-lg mx-auto">
              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-1">
                  <Label htmlFor="code">Campaign Code (Optional)</Label>
                  <Input id="code" value={formData.code} onChange={handleInputChange} placeholder="e.g., SUMMER_2025" />
                </div>
                <div className="col-span-1">
                  <Label htmlFor="priority">Priority</Label>
                  <Input id="priority" type="number" value={formData.priority} onChange={handleInputChange} placeholder="e.g., 10" />
                </div>
              </div>
              <div>
                <Label htmlFor="name">Name</Label>
                <Input id="name" value={formData.name} onChange={handleInputChange} minLength={5} required placeholder="e.g., Summer Weekend Bonus" />
              </div>
              <div>
                <Label htmlFor="description">Description (Optional)</Label>
                <Textarea id="description" value={formData.description} onChange={handleInputChange} placeholder="A brief description of the campaign." />
              </div>
              <div>
                <Label htmlFor="type">Primary Campaign Goal</Label>
                <Select value={formData.type} onValueChange={(val) => {
                  handleSelectChange(val, 'type');
                  setRewardConfig((prev: any) => ({ ...prev, type: val }));
                }}>
                  <SelectTrigger id="type">
                    <SelectValue placeholder="Select a campaign goal" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="points-multiplier">Points Multiplier</SelectItem>
                    <SelectItem value="bonus-points">Flat Bonus Points</SelectItem>
                    <SelectItem value="cashback">Cashback</SelectItem>
                    <SelectItem value="COUPON">Issue Coupon</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          )}
          {step === 2 && (
            <div className="space-y-6 max-w-lg mx-auto">
              <div>
                <Label className="text-base">Target Tiers</Label>
                <p className="text-sm text-muted-foreground mb-4">Select which loyalty tiers this campaign applies to. Leave empty for all tiers.</p>
                <div className="grid grid-cols-2 gap-4">
                  {tiers.map(tier => (
                    <div key={tier.id} className="flex items-center space-x-2">
                      <Checkbox
                        id={`tier-${tier.code}`}
                        checked={audience.tiers.includes(tier.code)}
                        onCheckedChange={() => toggleAudienceTier(tier.code)}
                      />
                      <Label htmlFor={`tier-${tier.code}`} className="font-normal">{tier.name}</Label>
                    </div>
                  ))}
                </div>
              </div>

              <div className="pt-4 border-t">
                <Label className="text-base">Points Requirement</Label>
                <p className="text-sm text-muted-foreground mb-4">Target members based on their current points balance.</p>
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="minPoints">Min Points</Label>
                    <Input
                      id="minPoints"
                      type="number"
                      value={audience.minPoints}
                      onChange={(e) => setAudience((prev: any) => ({ ...prev, minPoints: e.target.value }))}
                      placeholder="e.g., 0"
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="maxPoints">Max Points</Label>
                    <Input
                      id="maxPoints"
                      type="number"
                      value={audience.maxPoints}
                      onChange={(e) => setAudience((prev: any) => ({ ...prev, maxPoints: e.target.value }))}
                      placeholder="e.g., 10000"
                    />
                  </div>
                </div>
              </div>

              <div className="text-sm font-semibold text-blue-600 dark:text-blue-400 pt-4 flex items-center gap-2">
                <IconBolt className="h-4 w-4" />
                Estimated reach: {audience.tiers.length > 0 ? audience.tiers.join(', ') : 'All tiers'}
                {audience.minPoints || audience.maxPoints ? ` with points between ${audience.minPoints || 0} and ${audience.maxPoints || '∞'}` : ''}
              </div>
            </div>
          )}
          {step === 3 && (
            <div className="space-y-4 max-w-4xl mx-auto">
              {/* Toggle between Simple and Advanced Rule Builder */}
              <div className="flex items-center justify-between p-4 bg-black/40 border border-white/5 rounded-xl shadow-inner">
                <div>
                  <p className="text-sm font-bold text-amber-100/90 tracking-wide">Rule Builder Mode</p>
                  <p className="text-[11px] text-amber-100/40">Switch between simple and professional constructs</p>
                </div>
                <Button
                  className={`transition-all duration-300 ${useAdvancedRuleBuilder
                    ? 'bg-amber-600 hover:bg-amber-700 text-white border-amber-500 shadow-[0_0_15px_rgba(217,119,6,0.3)]'
                    : 'bg-amber-900/40 hover:bg-amber-900/60 text-amber-100 border-amber-600/30'
                    }`}
                  size="sm"
                  onClick={() => {
                    if (!useAdvancedRuleBuilder) {
                      // Convert simple rules to RuleGroup when switching to advanced
                      const group = createEmptyRuleGroup();
                      group.combinator = conditionOperator === 'and' ? 'AND' : 'OR';
                      if (rules.length > 0 && rules[0].field) {
                        group.conditions = rules.map((r: any) => ({
                          field: r.field,
                          operator: r.op as any,
                          value: r.value
                        }));
                      }
                      setRuleGroup(group);
                    }
                    setUseAdvancedRuleBuilder(!useAdvancedRuleBuilder);
                  }}
                >
                  {useAdvancedRuleBuilder ? 'Using Advanced Builder' : 'Use Advanced Builder'}
                </Button>
              </div>

              {/* Advanced Rule Builder */}
              {useAdvancedRuleBuilder ? (
                <div className="p-6 rounded-2xl bg-[#0c0800] border border-amber-900/20 shadow-2xl">
                  <RuleBuilder value={ruleGroup} onChange={setRuleGroup} />
                </div>
              ) : (
                /* Simple Rule Builder (existing) */
                <div className="p-4 border rounded-lg bg-slate-50 dark:bg-slate-900/50">
                  <div className="flex items-center space-x-2 mb-4">
                    <p>Match</p>
                    <Select value={conditionOperator} onValueChange={(v) => setConditionOperator(v as any)}>
                      <SelectTrigger className="w-auto"><SelectValue /></SelectTrigger>
                      <SelectContent><SelectItem value="and">ALL (AND)</SelectItem><SelectItem value="or">ANY (OR)</SelectItem></SelectContent>
                    </Select>
                    <p>of these conditions:</p>
                  </div>

                  <div className="space-y-3 mt-4">
                    {rules.map((rule: any, index: number) => (
                      <div key={index} className="flex items-start gap-2">
                        <div className="flex-1">
                          <Select value={rule.field} onValueChange={(v) => updateRule(index, 'field', v)}>
                            <SelectTrigger><SelectValue placeholder="Select field..." /></SelectTrigger>
                            <SelectContent>
                              {Object.entries(FIELDS).map(([group, opts]) => (
                                <SelectGroup key={group}>
                                  <SelectLabel>{group}</SelectLabel>
                                  {Object.entries(opts).map(([key, label]) => (
                                    <SelectItem key={key} value={key}>{label}</SelectItem>
                                  ))}
                                </SelectGroup>
                              ))}
                            </SelectContent>
                          </Select>
                        </div>
                        <div className="w-24">
                          <Select value={rule.op} onValueChange={(v) => updateRule(index, 'op', v)}>
                            <SelectTrigger><SelectValue /></SelectTrigger>
                            <SelectContent>
                              <SelectItem value="eq">=</SelectItem>
                              <SelectItem value="gte">&gt;=</SelectItem>
                              <SelectItem value="lte">&lt;=</SelectItem>
                              <SelectItem value="contains">contains</SelectItem>
                            </SelectContent>
                          </Select>
                        </div>
                        <div className="flex-1">
                          <Input value={rule.value} onChange={(e) => updateRule(index, 'value', e.target.value)} placeholder="Value..." />
                        </div>
                        <Button variant="ghost" size="icon" onClick={() => removeRule(index)} disabled={rules.length === 1}>
                          <IconTrash className="h-4 w-4" />
                        </Button>
                      </div>
                    ))}
                    <Button variant="outline" size="sm" onClick={addRule} className="mt-2">
                      <IconPlus className="h-4 w-4 mr-2" /> Add Condition
                    </Button>
                  </div>
                </div>
              )}
            </div>
          )}
          {step === 4 && (
            <div className="space-y-6 max-w-lg mx-auto">
              <div className="p-4 border rounded-lg bg-blue-50/50 dark:bg-blue-900/10 border-blue-100 dark:border-blue-900">
                <p className="text-sm text-blue-800 dark:text-blue-300">
                  {rewardConfig.type === 'points-multiplier' && "Member's earned points for the transaction will be multiplied by this value."}
                  {rewardConfig.type === 'bonus-points' && "Member's will receive a flat amount of bonus points for the qualifying event."}
                  {rewardConfig.type === 'cashback' && "A percentage of the transaction value will be credited to the cashback wallet."}
                  {rewardConfig.type === 'COUPON' && "Member will be issued the specified coupon code."}
                </p>
              </div>

              {rewardConfig.type === 'points-multiplier' && (
                <div className="space-y-2">
                  <Label>Multiplier Value</Label>
                  <Input
                    type="number"
                    step="0.1"
                    value={rewardConfig.value}
                    onChange={(e) => setRewardConfig((prev: any) => ({ ...prev, value: e.target.value }))}
                    placeholder="e.g., 2.0"
                  />
                </div>
              )}

              {rewardConfig.type === 'bonus-points' && (
                <div className="space-y-2">
                  <Label>Bonus Points Amount</Label>
                  <Input
                    type="number"
                    value={rewardConfig.points}
                    onChange={(e) => setRewardConfig((prev: any) => ({ ...prev, points: e.target.value }))}
                    placeholder="e.g., 500"
                  />
                </div>
              )}

              {rewardConfig.type === 'cashback' && (
                <div className="space-y-4">
                  <div className="space-y-2">
                    <Label>Cashback Percentage (%)</Label>
                    <Input
                      type="number"
                      value={rewardConfig.value}
                      onChange={(e) => setRewardConfig((prev: any) => ({ ...prev, value: e.target.value }))}
                      placeholder="e.g., 5"
                    />
                  </div>
                </div>
              )}

              {rewardConfig.type === 'COUPON' && (
                <div className="space-y-2">
                  <Label>Coupon Code</Label>
                  <Input
                    value={rewardConfig.value}
                    onChange={(e) => setRewardConfig((prev: any) => ({ ...prev, value: e.target.value }))}
                    placeholder="e.g., WELCOME50"
                  />
                  <p className="text-[10px] text-muted-foreground">The code of the Coupon definition to issue (must exist in system).</p>
                </div>
              )}
            </div>
          )}
          {step === 5 && (
            <div className="space-y-6 max-w-lg mx-auto">
              <div>
                <Label className="text-base">Validity Period</Label>
                <div className="grid grid-cols-2 gap-4 mt-2">
                  <div>
                    <Label htmlFor="validFrom">Start Date & Time</Label>
                    <Input id="validFrom" value={formData.validFrom} onChange={handleInputChange} type="datetime-local" required />
                  </div>
                  <div>
                    <Label htmlFor="validTo">End Date & Time (Optional)</Label>
                    <Input id="validTo" value={formData.validTo} onChange={handleInputChange} type="datetime-local" />
                  </div>
                </div>
              </div>

              <div className="pt-4 border-t">
                <Label className="text-base">Campaign Caps (Optional)</Label>
                <div className="grid grid-cols-2 gap-4 mt-2">
                  <div className="space-y-1">
                    <Label htmlFor="maxTotalRewards">Total Rewards Cap</Label>
                    <Input
                      id="maxTotalRewards"
                      type="number"
                      value={formData.maxTotalRewards}
                      onChange={handleInputChange}
                      placeholder="e.g., 5000"
                    />
                    <p className="text-[10px] text-muted-foreground">Max times this campaign can be used in total.</p>
                  </div>
                  <div className="space-y-1">
                    <Label htmlFor="maxPerCustomer">Per Customer Cap</Label>
                    <Input
                      id="maxPerCustomer"
                      type="number"
                      value={formData.maxPerCustomer}
                      onChange={handleInputChange}
                      placeholder="e.g., 5"
                    />
                    <p className="text-[10px] text-muted-foreground">Max times a single customer can use this.</p>
                  </div>
                </div>
              </div>
            </div>
          )}
          {step === 6 && (
            <div className="space-y-4 max-w-lg mx-auto">
              <Card className="bg-slate-50 dark:bg-slate-800/50">
                <CardHeader><CardTitle>{formData.name || 'Untitled Campaign'}</CardTitle></CardHeader>
                <CardContent className="space-y-4 text-sm">
                  <div>
                    <p className="font-semibold">Description:</p>
                    <p>{formData.description || 'No description provided.'}</p>
                  </div>
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <p className="font-semibold">Goal:</p>
                      <p className="capitalize">{formData.type.replace('-', ' ')}</p>
                    </div>
                    <div>
                      <p className="font-semibold">Match Logic:</p>
                      <p>{conditionOperator.toUpperCase()} of {rules.length} rule(s)</p>
                    </div>
                  </div>
                  <div>
                    <p className="font-semibold">Reward Config:</p>
                    <p>{rewardConfig.type}: {rewardConfig.value || rewardConfig.points || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="font-semibold">Schedule:</p>
                    <p>{new Date(formData.validFrom).toLocaleString()} to {new Date(formData.validTo).toLocaleString()}</p>
                  </div>
                </CardContent>
              </Card>
            </div>
          )}
        </CardContent>
        <CardFooter className="flex justify-between">
          <Button variant="ghost" onClick={() => router.push('/campaigns')}>Cancel</Button>
          <div className="flex gap-2">
            <Button variant="outline" onClick={prevStep} disabled={step === 1 || isSubmitting}>Previous</Button>
            {step === steps.length && (
              <Button
                variant="secondary"
                onClick={() => handleSubmit('DRAFT')}
                disabled={isSubmitting}
              >
                {isSubmitting ? <IconLoader2 className="animate-spin h-4 w-4 mr-2" /> : null}
                Save Draft
              </Button>
            )}
            {step === steps.length ? (
              <Button
                onClick={() => handleSubmit('ACTIVE')}
                disabled={isSubmitting}
              >
                {isSubmitting ? <IconLoader2 className="animate-spin h-4 w-4 mr-2" /> : null}
                Activate
              </Button>
            ) : (
              <Button onClick={nextStep}>Next</Button>
            )}
          </div>
        </CardFooter>
      </Card>
    </div>
  );
}
