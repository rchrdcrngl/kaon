FROM openjdk:8-jdk-alpine
ARG JAR_FILE=target/*.jar
COPY ${JAR_FILE} srvcregistry.jar
ENTRYPOINT ["java", "-Xmx2048M", "-jar", "/srvcregistry.jar"]