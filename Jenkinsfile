pipeline {
    agent any
    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/wyvernk/FinalHairyPotterGroup3SSD.git'
            }
        }

        stage('Code Quality Check via SonarQube') {
            steps {
                script {
                    def scannerHome = tool 'SonarQube'; // Ensure 'SonarQube' is the correct tool name configured in Jenkins
                    withSonarQubeEnv('SonarQube') { // Ensure 'SonarQube' is the correct SonarQube server name configured in Jenkins
                        sh "${scannerHome}/bin/sonar-scanner -Dsonar.projectKey=OWASP -Dsonar.sources=."
                    }
                }
            }
        }
    }
    
    post {
        always {
            recordIssues enabledForFailure: true, tool: sonarQube()
        }
    }
}
