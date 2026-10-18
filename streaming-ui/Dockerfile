FROM node:22-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:alpine

RUN addgroup -S appgroup \
    && adduser -S -D -H -G appgroup appuser \
    && sed -i '/^[[:space:]]*user[[:space:]]\+nginx;/d' /etc/nginx/nginx.conf \
    && sed -i 's|^pid .*|pid /tmp/nginx.pid;|' /etc/nginx/nginx.conf \
    && mkdir -p /var/cache/nginx /tmp/nginx

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/streaming-ui/browser /usr/share/nginx/html

RUN chown -R appuser:appgroup \
    /var/cache/nginx \
    /tmp/nginx \
    /usr/share/nginx/html \
    /etc/nginx

USER appuser

EXPOSE 8080
