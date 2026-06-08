FROM node:20-alpine AS build-step
WORKDIR /app

RUN apk add --no-cache curl openjdk17-jre

COPY package*.json ./
RUN npm install

COPY . .

# RUN npm run generate:api:prod

RUN npm run build -- --configuration=production

FROM nginx:stable-alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build-step /app/dist/GuayaquilBank.Frontend/browser /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
