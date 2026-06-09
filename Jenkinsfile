pipeline {
    agent any

    environment {
        PATH = "/opt/homebrew/bin:/usr/local/bin:/usr/bin:/bin:${PATH}"
        ACR     = 'anishfullstackacr'          // your ACR name (no .azurecr.io)
        RG      = 'fullstack-rg'
        AKS     = 'fullstack-aks'
        IMAGE   = 'product-api'
        AZ_CLIENT_ID     = credentials('azure-client-id')
        AZ_CLIENT_SECRET = credentials('azure-client-secret')
        AZ_TENANT_ID     = credentials('azure-tenant-id')
    }

    stages {
        stage('Checkout') {
            steps { checkout scm }
        }

        stage('Build image') {
            steps {
                sh 'docker build --platform linux/amd64 -t ${ACR}.azurecr.io/${IMAGE}:${BUILD_NUMBER} -t ${ACR}.azurecr.io/${IMAGE}:latest .'
            }
        }

        stage('Login to Azure') {
            steps {
                sh '''
                    az login --service-principal \
                        -u $AZ_CLIENT_ID -p $AZ_CLIENT_SECRET --tenant $AZ_TENANT_ID
                    az acr login -n ${ACR}
                '''
            }
        }

        stage('Push to ACR') {
            steps {
                sh '''
                    docker push ${ACR}.azurecr.io/${IMAGE}:${BUILD_NUMBER}
                    docker push ${ACR}.azurecr.io/${IMAGE}:latest
                '''
            }
        }

        stage('Deploy to AKS') {
            steps {
                sh '''
                    az aks get-credentials -n ${AKS} -g ${RG} --overwrite-existing

                    # Apply MySQL + API manifests (idempotent: safe to re-run).
                    # Substitute the ACR name placeholder at apply time.
                    sed "s/<ACR_NAME>/${ACR}/g" k8s/02-api.yaml > /tmp/02-api.yaml
                    kubectl apply -f k8s/01-mysql.yaml
                    kubectl apply -f /tmp/02-api.yaml

                    # Roll the API to the freshly built tag.
                    kubectl set image deployment/product-api \
                        product-api=${ACR}.azurecr.io/${IMAGE}:${BUILD_NUMBER}
                    kubectl rollout status deployment/product-api --timeout=120s
                '''
            }
        }
    }

    post {
        success { echo "product-api ${BUILD_NUMBER} deployed to AKS." }
        failure { echo 'product-api pipeline failed.' }
        always  { sh 'az logout || true' }
    }
}
