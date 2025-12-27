/**
 * Type definitions for the Rule Builder component
 */

/**
 * Logical combinator for rule groups
 */
export type Combinator = "AND" | "OR";

/**
 * Supported comparison operators
 */
export type Operator =
    | "gt"      // Greater than
    | "gte"     // Greater than or equal
    | "lt"      // Less than
    | "lte"     // Less than or equal
    | "eq"      // Equal
    | "neq"     // Not equal
    | "contains"   // String contains
    | "startsWith" // String starts with
    | "endsWith"   // String ends with
    | "in"         // Value in array
    | "notIn";     // Value not in array

/**
 * Field type for determining available operators and input type
 */
export type FieldType = "string" | "number" | "boolean" | "array" | "date";

/**
 * Metadata for a rule field
 */
export interface FieldDefinition {
    name: string;
    label: string;
    type: FieldType;
    category: "transaction" | "customer" | "product" | "other";
    description?: string;
    options?: Array<{ value: string; label: string }>; // For enum fields
}

/**
 * Individual rule condition (field-operator-value)
 */
export interface RuleCondition {
    field: string;
    operator: Operator;
    value: any;
}

/**
 * Rule group containing conditions and nested groups
 */
export interface RuleGroup {
    combinator: Combinator;
    conditions: RuleCondition[];
    groups: RuleGroup[];
}

/**
 * Available field definitions for the rule builder
 */
export const FIELD_DEFINITIONS: FieldDefinition[] = [
    // Transaction fields
    {
        name: "min_order_amount",
        label: "Order Amount",
        type: "number",
        category: "transaction",
        description: "Total transaction amount"
    },
    {
        name: "gross_value",
        label: "Gross Value",
        type: "number",
        category: "transaction",
        description: "Gross transaction value"
    },
    {
        name: "amount",
        label: "Amount",
        type: "number",
        category: "transaction",
        description: "Transaction amount"
    },
    {
        name: "history_tx_count",
        label: "Transaction Count",
        type: "number",
        category: "transaction",
        description: "Number of previous transactions"
    },
    {
        name: "history_tx_sum",
        label: "Total Spend",
        type: "number",
        category: "transaction",
        description: "Total amount spent historically"
    },

    // Customer fields
    {
        name: "CustomerTier",
        label: "Customer Tier",
        type: "string",
        category: "customer",
        description: "Customer loyalty tier",
        options: [
            { value: "BRONZE", label: "Bronze" },
            { value: "SILVER", label: "Silver" },
            { value: "GOLD", label: "Gold" },
            { value: "PLATINUM", label: "Platinum" }
        ]
    },
    {
        name: "PointsBalance",
        label: "Points Balance",
        type: "number",
        category: "customer",
        description: "Current points balance"
    },

    // Product fields
    {
        name: "Category",
        label: "Product Category",
        type: "string",
        category: "product",
        description: "Product category"
    },
    {
        name: "Sku",
        label: "Product SKU",
        type: "string",
        category: "product",
        description: "Product SKU code"
    },
    {
        name: "StoreId",
        label: "Store ID",
        type: "string",
        category: "product",
        description: "Store identifier"
    }
];

/**
 * Get operators available for a specific field type
 */
export function getOperatorsForFieldType(fieldType: FieldType): Operator[] {
    switch (fieldType) {
        case "number":
            return ["gt", "gte", "lt", "lte", "eq", "neq"];
        case "string":
            return ["eq", "neq", "contains", "startsWith", "endsWith"];
        case "array":
            return ["in", "notIn"];
        case "boolean":
            return ["eq", "neq"];
        case "date":
            return ["gt", "gte", "lt", "lte", "eq", "neq"];
        default:
            return ["eq", "neq"];
    }
}

/**
 * Get human-readable label for an operator
 */
export function getOperatorLabel(operator: Operator): string {
    const labels: Record<Operator, string> = {
        gt: "Greater than",
        gte: "Greater than or equal to",
        lt: "Less than",
        lte: "Less than or equal to",
        eq: "Equals",
        neq: "Not equals",
        contains: "Contains",
        startsWith: "Starts with",
        endsWith: "Ends with",
        in: "In",
        notIn: "Not in"
    };
    return labels[operator] || operator;
}

/**
 * Create an empty rule group
 */
export function createEmptyRuleGroup(): RuleGroup {
    return {
        combinator: "AND",
        conditions: [],
        groups: []
    };
}

/**
 * Converts a RuleGroup structure to Microsoft RulesEngine Workflow JSON format
 */
export function convertToMicrosoftRulesEngine(group: RuleGroup, campaignId: string): any[] {
    const rules = [
        {
            RuleName: "MainCondition",
            SuccessEvent: "qualifies",
            ErrorMessage: "Not eligible",
            Expression: buildExpression(group),
            Actions: {
                OnSuccess: {
                    Name: "OutputExpression",
                    Context: {
                        Expression: "Success"
                    }
                }
            }
        }
    ];

    return [
        {
            WorkflowName: `campaign_${campaignId}`,
            Rules: rules
        }
    ];
}

function buildExpression(group: RuleGroup): string {
    if (!group || (!group.conditions.length && !group.groups.length)) return "true";

    const expressions: string[] = [];

    // Process conditions
    for (const condition of group.conditions) {
        if (!condition.field) continue;
        const expr = mapConditionToExpression(condition);
        if (expr) expressions.push(expr);
    }

    // Process nested groups
    for (const subGroup of group.groups) {
        const expr = buildExpression(subGroup);
        if (expr && expr !== "true") expressions.push(`(${expr})`);
    }

    if (expressions.length === 0) return "true";
    if (expressions.length === 1) return expressions[0];

    const combinator = group.combinator === "OR" ? " || " : " && ";
    return expressions.join(combinator);
}

function mapConditionToExpression(c: RuleCondition): string {
    const field = c.field;
    const op = c.operator;
    const val = formatValue(c.value, field);

    // Map frontend field names to backend input models
    let inputField = `input.${field}`;
    if (field === "min_order_amount" || field === "amount") {
        inputField = "input.Amount";
    } else if (field === "gross_value") {
        inputField = "input.GrossValue";
    } else if (field === "history_tx_count") {
        inputField = "input.HistoryTxCount";
    } else if (field === "history_tx_sum") {
        inputField = "input.HistoryTxSum";
    } else if (field === "PointsBalance") {
        inputField = "input.PointsBalance";
    } else if (field === "CustomerTier") {
        inputField = "input.CustomerTier";
    }

    switch (op) {
        case "gt": return `${inputField} > ${val}`;
        case "gte": return `${inputField} >= ${val}`;
        case "lt": return `${inputField} < ${val}`;
        case "lte": return `${inputField} <= ${val}`;
        case "eq": return `${inputField} == ${val}`;
        case "neq": return `${inputField} != ${val}`;
        case "contains": return `${inputField}.Contains(${val})`;
        case "startsWith": return `${inputField}.StartsWith(${val})`;
        case "endsWith": return `${inputField}.EndsWith(${val})`;
        case "in": {
            const arr = Array.isArray(c.value) ? c.value : [c.value];
            const formattedArr = arr.map(v => formatValue(v, field)).join(", ");
            return `new[] { ${formattedArr} }.Contains(${inputField})`;
        }
        case "notIn": {
            const arr = Array.isArray(c.value) ? c.value : [c.value];
            const formattedArr = arr.map(v => formatValue(v, field)).join(", ");
            return `!new[] { ${formattedArr} }.Contains(${inputField})`;
        }
        default: return "true";
    }
}

function formatValue(val: any, fieldName: string): string {
    if (val === null || val === undefined || val === "") {
        const field = FIELD_DEFINITIONS.find(f => f.name === fieldName);
        if (field?.type === "number") return "0";
        if (field?.type === "boolean") return "false";
        return "\"\"";
    }

    const field = FIELD_DEFINITIONS.find(f => f.name === fieldName);

    if (field?.type === "number") {
        const num = parseFloat(val);
        return isNaN(num) ? "0" : num.toString();
    }

    if (typeof val === "boolean" || field?.type === "boolean") {
        return val.toString().toLowerCase();
    }

    return `"${val.toString().replace(/"/g, '\\"')}"`;
}

/**
 * Create an empty rule condition
 */
export function createEmptyRuleCondition(): RuleCondition {
    return {
        field: "",
        operator: "eq",
        value: null
    };
}
