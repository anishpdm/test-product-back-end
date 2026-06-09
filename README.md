# product-api

.NET 8 Web API + EF Core + MySQL. CRUD for Products.
**This repo owns the database** — its `k8s/` folder has both MySQL and the API manifests.

```
k8s/01-mysql.yaml   MySQL StatefulSet + PVC + Service
k8s/02-api.yaml     product-api Deployment + Service + Secret
Dockerfile          builds the API image
Jenkinsfile         build → ACR → apply manifests → rollout
```

## Local test
```bash
docker build --platform linux/amd64 -t product-api .
docker run -p 5000:8080 product-api
```
http://localhost:5000/api/products  ·  http://localhost:5000/swagger

Full setup (both repos + Jenkins + AKS): see **DEPLOYMENT.md**.
Pairs with the **product-frontend** repo.
