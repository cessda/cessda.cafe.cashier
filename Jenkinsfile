pipeline {
	options {
		ansiColor('xterm')
		buildDiscarder logRotator(artifactNumToKeepStr: '5', numToKeepStr: '10')
	}

	environment {
		project_name = "${GCP_PROJECT}"
		product_name = "cafe"
		module_name = "cashier"
		image_tag = "${docker_repo}/${product_name}-${module_name}:${env.BRANCH_NAME}-${env.BUILD_NUMBER}"
		HOME = '/tmp'
		cluster = "management-cluster"
	}

	agent any

	stages {
		stage('Set up gcloud') {
			steps {
				sh("gcloud config set project ${project_name}")
				sh("gcloud container clusters get-credentials ${cluster} --zone=${zone}")
			}
		}
		stage('Build and Test Cashier') {
			agent {
				docker {
					image 'dotnet/core/sdk:2.2-stretch'
					registryUrl 'https://mcr.microsoft.com'
					reuseNode true
				}
			}
			stages {
				stage('Test Cashier') {
					steps {
						dir('./Cashier/') {
							sh 'dotnet test --logger:trx'
						}
					}
				}
				post {
					always {
						dir('./Cashier/') {
							mstest()
						}
					}
				}
			stage('Build Cashier') {
				steps {
					sh 'dotnet build "Cashier/Cashier.csproj" -c Release -o ./publish'
				}
				post {
					always {
						archiveArtifacts './Cashier/publish/**'
					}
				}
			}
			stage('Publish Cashier') {
				steps {
					sh 'dotnet publish "Cashier/Cashier.csproj" -c Release -o ./publish'
				}
			}
		}
		stage('Build Docker Container') {
			steps {
				sh "docker build -t ${image_tag} ."
			}
		}
		stage('Push Docker Container') {
			steps {
				sh("gcloud auth configure-docker")
				sh("docker push ${image_tag}")
				sh("gcloud container images add-tag ${image_tag} ${docker_repo}/${product_name}-${module_name}:${env.BRANCH_NAME}-latest")
			}
		}
	}
}