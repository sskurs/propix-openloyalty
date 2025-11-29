// Fresh implementation of the Tiers page - v2
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { IconShield } from '@tabler/icons-react';

const tiers = [
  {
    name: 'BRONZE',
    color: '#CD7F32',
    threshold: 0,
    qualification: '12 months rolling',
    perks: ['Basic support', 'Standard earning rate'],
  },
  {
    name: 'SILVER',
    color: '#C0C0C0',
    threshold: 2000,
    qualification: '12 months rolling',
    perks: ['Priority support', '1.2x earning rate'],
  },
  {
    name: 'GOLD',
    color: '#D4AF37',
    threshold: 5000,
    qualification: '12 months rolling',
    perks: ['Dedicated support', '1.5x earning rate', 'Birthday reward'],
  },
  {
    name: 'PLATINUM',
    color: '#2B2B2B',
    threshold: 10000,
    qualification: '12 months rolling',
    perks: ['24/7 dedicated support', '2x earning rate', 'Exclusive event access'],
  },
];

const TierBadge = ({ name, color }: { name: string; color: string }) => (
  <div className="relative w-24 h-28">
    <IconShield className="w-full h-full" style={{ fill: color, color: color }} />
    <div className="absolute inset-0 flex flex-col items-center justify-center text-white">
      <span className="text-xs font-bold -mt-2 tracking-wider uppercase">{name}</span>
    </div>
  </div>
);

export default function TiersPage() {
  return (
    <div>
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-2xl font-bold">Tiers</h1>
        <Button>+ New Tier</Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
        {tiers.map((tier) => (
          <Card key={tier.name} className="flex flex-col text-center">
            <CardHeader className="items-center">
                <TierBadge name={tier.name} color={tier.color} />
            </CardHeader>
            <CardContent className="flex-grow space-y-4">
                <CardTitle className="text-2xl font-bold">{tier.name}</CardTitle>
                <CardDescription>
                    Reach <span className="font-semibold text-foreground">{tier.threshold.toLocaleString()}</span> points in <span className="font-semibold text-foreground">{tier.qualification}</span>.
                </CardDescription>
                <ul className="text-sm text-muted-foreground list-disc list-inside text-left mx-auto max-w-max">
                    {tier.perks.map(perk => <li key={perk}>{perk}</li>)}
                </ul>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
