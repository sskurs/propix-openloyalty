# ===============================
# Stage 1: Builder
# ===============================
FROM node:20-alpine AS builder

WORKDIR /app

# Copy dependency files first (cache-friendly)
COPY package.json package-lock.json ./

# Deterministic install
RUN npm ci --legacy-peer-deps

# Copy source
COPY . .

# Build Next.js
RUN npm run build

# ===============================
# Stage 2: Runner
# ===============================
FROM node:20-alpine AS runner

WORKDIR /app
ENV NODE_ENV=production

# Copy minimal standalone output
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public

EXPOSE 3000

CMD ["node", "server.js"]
