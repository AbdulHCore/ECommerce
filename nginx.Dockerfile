FROM nginx

COPY ApiGateways/nginx/nginx.local.conf /etc/nginx/nginx.conf

# This Certificate generated in powershell based on above conf file (Check the word notes for details)
COPY ApiGateways/nginx/id-local.crt /etc/ssl/certs/id-local.eshopping.com.crt 

# This key generated in powershell based on above conf file (Check the word notes for details)
COPY ApiGateways/nginx/id-local.key /etc/ssl/private/id-local.eshopping.com.key