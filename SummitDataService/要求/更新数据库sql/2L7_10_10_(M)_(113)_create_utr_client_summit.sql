if exists(select 1 from sysobjects where name = 'tr_u_client_summit')
drop trigger tr_u_client_summit
go
create trigger "tr_u_client_summit" 
after update of ccode,ename,cname,addr,city,
stat,zip,tele,cust_bill_nm,cust_cntct,cust_email,
cust_shp_nm,cust_shp_addr,cust_shp_city,cust_shp_state,cust_shp_zip,
cust_sales_no,bill_cnm,shp_cnm,cust_lz_no order 2 on
DBA.client
referencing new as new_row
for each row
begin
	if not exists (select 1 from dba.t_summit_data_out where do_type = 3 and do_key = new_row.ccode) then
		insert into DBA.t_summit_data_out (do_id,do_type,do_key,do_crt_time) values (null,3,new_row.ccode,getdate())
  	end if
end;
insert t_upgrade_info values (getdate(),'2L7_10_10_(M)_(113)_create_utr_client_summit',null)