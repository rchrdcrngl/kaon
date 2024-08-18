FROM openjdk:8-jdk-alpine
ARG JAR_FILE=target/*.jar
COPY ${JAR_FILE} users-srvc.jar
ENTRYPOINT ["java", "-Xmx2048M", "-jar", "/users-srvc.jar"]